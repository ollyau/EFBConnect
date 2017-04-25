using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SilverLightPolicyServer
{
    // Encapsulate and manage state for a single connection from a client    
    class PolicyConnection
    {
        private Socket _connection;
        private byte[] _buffer; // buffer to receive the request from the client               
        private int _received;
        private byte[] _policy; // the policy to return to the client         

        // the request that we're expecting from the client        
        private static string _policyRequestString = "<policy-file-request/>";
        public PolicyConnection(Socket client, byte[] policy)
        {
            _connection = client;
            _policy = policy;

            _buffer = new byte[_policyRequestString.Length];
            _received = 0;

            try
            {
                // receive the request from the client                
                _connection.BeginReceive(_buffer, 0, _policyRequestString.Length, SocketFlags.None,
                    new AsyncCallback(OnReceive), null);
            }
            catch (SocketException)
            {
                _connection.Close();
            }
        }

        // Called when we receive data from the client        
        private void OnReceive(IAsyncResult res)
        {
            try
            {
                _received += _connection.EndReceive(res);

                // if we haven't gotten enough for a full request yet, receive again                
                if (_received < _policyRequestString.Length)
                {
                    _connection.BeginReceive(_buffer, _received, _policyRequestString.Length - _received,
                        SocketFlags.None, new AsyncCallback(OnReceive), null);
                    return;
                }

                // make sure the request is valid                
                string request = System.Text.Encoding.UTF8.GetString(_buffer, 0, _received);
                if (StringComparer.InvariantCultureIgnoreCase.Compare(request, _policyRequestString) != 0)
                {
                    _connection.Close();
                    return;
                }

                // send the policy                
                Console.Write("Sending policy...\n");
                _connection.BeginSend(_policy, 0, _policy.Length, SocketFlags.None,
                    new AsyncCallback(OnSend), null);
            }
            catch (SocketException)
            {
                _connection.Close();
            }
        }

        // called after sending the policy to the client; close the connection.        
        public void OnSend(IAsyncResult res)
        {
            try
            {
                _connection.EndSend(res);
            }
            finally
            {
                _connection.Close();
            }
        }
    }

    // Listens for connections on port 943 and dispatches requests to a PolicyConnection    
    class PolicyServer
    {
        private Socket _listener;
        private byte[] _policy;

        // pass in the path of an XML file containing the socket policy        
        public PolicyServer(string policyFileText)
        {
            // Load the policy file string into byte array
            _policy = new byte[policyFileText.Length];
            System.Text.Encoding.UTF8.GetBytes(policyFileText).CopyTo(_policy, 0);

            // Create the Listening Socket            
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.SetSocketOption(SocketOptionLevel.Tcp, (SocketOptionName)SocketOptionName.NoDelay, 0);
            _listener.Bind(new IPEndPoint(IPAddress.Any, 943));
            _listener.Listen(10);
            _listener.BeginAccept(new AsyncCallback(OnConnection), null);
        }

        // Called when we receive a connection from a client        
        public void OnConnection(IAsyncResult res)
        {
            Socket client = null;
            try
            {
                client = _listener.EndAccept(res);
            }
            catch (SocketException)
            {
                return;
            }

            // handle this policy request with a PolicyConnection            
            PolicyConnection pc = new PolicyConnection(client, _policy);

            // look for more connections            
            _listener.BeginAccept(new AsyncCallback(OnConnection), null);
        }

        public void Close()
        {
            _listener.Close();
        }
    }

    public class Program
    {
        static string PolicyFileText = 
            "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
            "<access-policy>" +
            "    <cross-domain-access>" +
            "        <policy>" +
            "            <allow-from>" +
            "                <domain uri=\"*\"/>" +
            "            </allow-from>" +
            "            <grant-to>" +
            "                <socket-resource port=\"4502-4534\" protocol=\"tcp\"/>" +
            "            </grant-to>" +
            "        </policy>" +
            "    </cross-domain-access>" +
            "</access-policy>";

        static void Main()
        {
            Console.Write("Starting...\n");
            string strCWD = System.IO.Directory.GetCurrentDirectory();
            PolicyServer ps = new PolicyServer(PolicyFileText);
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
    }
}
