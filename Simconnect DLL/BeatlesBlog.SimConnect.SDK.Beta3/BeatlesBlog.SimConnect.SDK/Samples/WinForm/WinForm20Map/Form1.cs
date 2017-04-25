using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.MapPoint.PlugIns;
using Microsoft.MapPoint.Rendering3D;
using Microsoft.MapPoint.Rendering3D.NavigationControl;
using Microsoft.MapPoint.Rendering3D.Utility;

using BeatlesBlog.SimConnect;

namespace WinFormMap
{
    public partial class Form1 : Form
    {
        private GlobeControl globeControl;
        private PlugInLoader loader;
        private MyCameraController controller;

        private SimConnect sc = null;
        private string LastServerName = "localhost";
        private string LastServerPort = "4504";

        enum Requests
        {
            UserPositionOrienation,
        }

        public Form1()
        {
            InitializeComponent();

            this.Closing += new CancelEventHandler(Form1_Closing);

            // create the control and set Forms properties.
            this.globeControl = new GlobeControl();
            this.SuspendLayout();
            this.globeControl.Location = new System.Drawing.Point(0, 0);
            this.globeControl.Name = "globeControl";
//            this.globeControl.Size = this.ClientSize;
//            this.globeControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            this.globeControl.Dock = DockStyle.Fill;
            this.globeControl.TabIndex = 0;
//            this.globeControl.SendToBack(); // we want the button to be on top

            this.toolStripContainer1.ContentPanel.Controls.Add(this.globeControl);
            this.ResumeLayout(false);

            this.loader = PlugInLoader.CreateLoader(this.globeControl.Host);

            // at this stage, it is safe to attach events to the control, but otherwise wait until Initialized.
            this.globeControl.Host.RenderEngine.Initialized += new EventHandler(Initialized);
        }

        void Form1_Closing(object sender, CancelEventArgs e)
        {
            if (sc != null)
            {
                sc.Close();
                controller.StopCamera();
                sc = null;
            }
        }

        private void Initialized(object sender, EventArgs e)
        {
            // at this point, the control is fully initialized and we can interact with it without worry.

            // set various data sources, here for elevation data, terrain data, and model data.
            this.globeControl.Host.DataSources.Add(new DataSourceLayerData("Elevation", "Elevation", @"http://go.microsoft.com/fwlink/?LinkID=98774", DataSourceUsage.ElevationMap));
            this.globeControl.Host.DataSources.Add(new DataSourceLayerData("Texture", "Texture", @"http://go.microsoft.com/fwlink/?LinkID=98771", DataSourceUsage.TextureMap));
            this.globeControl.Host.DataSources.Add(new DataSourceLayerData("Models", "Models", @"http://go.microsoft.com/fwlink/?LinkID=98775", DataSourceUsage.Model));

            // set some visual display variables
            this.globeControl.Host.WorldEngine.Environment.AtmosphereDisplay = Microsoft.MapPoint.Rendering3D.Atmospherics.EnvironmentManager.AtmosphereStyle.Regular;
            this.globeControl.Host.WorldEngine.Environment.CelestialDisplay = Microsoft.MapPoint.Rendering3D.Atmospherics.EnvironmentManager.CelestialStyle.Regular;
            this.globeControl.Host.WorldEngine.Environment.LocalWeatherEnabled = true;
            this.globeControl.Host.WorldEngine.Environment.ShadowsEnabled = true;
            this.globeControl.Host.WorldEngine.EnableInertia = false;
            this.globeControl.Host.WorldEngine.ShowBuildings = true;
            this.globeControl.Host.WorldEngine.ShowBuildingTextures = true;
            this.globeControl.Host.WorldEngine.ShowImmersiveAdvertising = false;
            this.globeControl.Host.RenderEngine.Graphics.Settings.UseAnisotropicFiltering = true;

            this.globeControl.Host.WorldEngine.Environment.SunPosition = null; // this means to use real-time lighting

            controller = new MyCameraController(this.globeControl.Host.CameraControllers.Default);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void sc_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            this.globeControl.Host.CameraControllers.Current = controller;
            sc.RequestDataOnUserSimObject(Requests.UserPositionOrienation, SIMCONNECT_PERIOD.SIM_FRAME, controller);
        }

        private void connectLocalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sc = new SimConnect(this);
            sc.OnRecvOpen += new SimConnect.RecvOpenEventHandler(sc_OnRecvOpen);

            sc.Open("WinFormMap");
            
            this.connectToolStripMenuItem.Visible = false;
            this.disconnectToolStripMenuItem.Visible = true;
        }

        private void connectCustomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SimConnectConfigure scConfig = new SimConnectConfigure(LastServerName, LastServerPort);
            if (scConfig.ShowDialog(this) == DialogResult.OK)
            {
                sc = new SimConnect(this);
                sc.OnRecvOpen += new SimConnect.RecvOpenEventHandler(sc_OnRecvOpen);

                sc.Open("WinFormMap", scConfig.ServerName, scConfig.ServerPortInt);

                LastServerName = scConfig.ServerName;
                LastServerPort = scConfig.ServerPort;

                this.connectToolStripMenuItem.Visible = false;
                this.disconnectToolStripMenuItem.Visible = true;
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sc.Close();

            controller.StopCamera();

            this.connectToolStripMenuItem.Visible = true;
            this.disconnectToolStripMenuItem.Visible = false;

            sc = null;
        }

        private void connectToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            connectLocalToolStripMenuItem.Enabled = SimConnect.IsLocalRunning();
        }
    }

    [DataStruct]
    public class MyCameraController : Microsoft.MapPoint.Rendering3D.Cameras.CameraController<Microsoft.MapPoint.Rendering3D.Cameras.PredictiveCamera>
    {
        [DataItem("PLANE LATITUDE", "radians")]
        public double fLat = 38.276 * Microsoft.MapPoint.Constants.RadiansPerDegree;

        [DataItem("PLANE LONGITUDE", "radians")]
        public double fLon = -77.491 * Microsoft.MapPoint.Constants.RadiansPerDegree;

        [DataItem("PLANE ALTITUDE", "meters")]
        public double fAlt = 6000.0 * Microsoft.MapPoint.Constants.MetersPerFoot;

        [DataItem("PLANE PITCH DEGREES", "radians")]
        public double fPitch = -0.5;

        [DataItem("PLANE BANK DEGREES", "radians")]
        public double fBank = 0.0;

        [DataItem("PLANE HEADING DEGREES TRUE", "radians")]
        public double fHeading = -1.2;


        public MyCameraController(Microsoft.MapPoint.Rendering3D.Cameras.CameraController<Microsoft.MapPoint.Rendering3D.Cameras.PredictiveCamera> next) 
            : base()
        {
            this.Next = next;
        }

        public void StopCamera()
        {
            this.HasArrived = true;
        }

        public override void Activate()
        {
            base.Activate();
            this.HasArrived = false;
        }

        public override void MoveCamera(Microsoft.MapPoint.Rendering3D.Scene.SceneState sceneState)
        {
            LatLonAlt lla = new LatLonAlt(fLat, fLon, fAlt);
            lla.Altitude += (lla.Altitude - lla.AltitudeAboveSeaLevel);
            this.Camera.Viewpoint.Position.Location = lla;
            this.Camera.Viewpoint.LocalOrientation.RollPitchYaw = new Microsoft.MapPoint.Geometry.VectorMath.RollPitchYaw(-fBank, -fPitch, Microsoft.MapPoint.Constants.TwoPI - fHeading);
        }
    }
}
