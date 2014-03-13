using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Fleck;
using Microsoft.Kinect;
using System.Windows;
using Newtonsoft.Json;

namespace Kinect_server
{
    class Program
    {

        static List<IWebSocketConnection> _clients = new List<IWebSocketConnection>();
        static bool _serverInitialized = false;

        static private KinectSensor _kinectSensor;

        static private long _startTime = 0;
        static private uint framesSinceUpdate = 0;
        static private Stopwatch stopwatch = null;
        static private DateTime _nextStatusUpdate = DateTime.MinValue;

        static private Body[] bodies = null;
        static private BodyFrameReader reader = null;

        static private CoordinateMapper _coordinateMapper;


        static void Main(string[] args)
        {
            try
            {
                InitilizeKinect();
                InitializeServer();

            }
            catch (Exception)
            {
                
                //errorcillo
            }
            

        }

        private static void InitializeServer()
        {
            var server = new WebSocketServer("ws://localhost:8181");

            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("Connected to " + socket.ConnectionInfo.ClientIpAddress);
                    _clients.Add(socket);
                    InitilizeKinect();

                };

                socket.OnClose = () =>
                {
                    Console.WriteLine("Disconnected from " + socket.ConnectionInfo.ClientIpAddress);
                    _clients.Remove(socket);
                };

                socket.OnMessage = message =>
                {
                    Console.WriteLine(message);
                };
            });

            _serverInitialized = true;

            Console.ReadLine();
        }

        private static void InitilizeKinect()
        {

            stopwatch = new Stopwatch();

            _kinectSensor = KinectSensor.Default;
            

            if (_kinectSensor != null)
            {
                
                // get the coordinate mapper
                _coordinateMapper = _kinectSensor.CoordinateMapper;

                // open the sensor
                _kinectSensor.Open();



                int displayWidth;
                int displayHeight;
               



                // get the depth (display) extents
                FrameDescription frameDescription = _kinectSensor.DepthFrameSource.FrameDescription;


                displayWidth = frameDescription.Width;
                displayHeight = frameDescription.Width;
                bodies = new Body[_kinectSensor.BodyFrameSource.BodyCount];


                // open the reader for the body frames
                reader = _kinectSensor.BodyFrameSource.OpenReader(); // using (var frame = e.OpenSkeletonFrame())


                //descomentar cuando esté la kinect
                if (reader != null)
                {
                    reader.FrameArrived += Reader_FrameArrived;
                }

                //mock kinect
               // GiveMeBodies();

              

                // convert the joint points to depth (display) space
                //mockeamos un cuerpo un poco deforme
                //List<Joint> joints = new List<Joint>();

                //Joint j = new Joint();
                //j.JointType = JointType.AnkleLeft;
                //j.Position = new CameraSpacePoint();
                //j.Position.X = 500;
                //j.Position.Y = 200;
                //j.TrackingState = TrackingState.Tracked;
                //joints.Add(j);


 
                //j.JointType = JointType.AnkleLeft;
                //j.Position = new CameraSpacePoint();
                //j.Position.X = 30;
                //j.Position.Y = 50;
                //j.TrackingState = TrackingState.Tracked;
                //joints.Add(j);


  
                //j.JointType = JointType.AnkleLeft;
                //j.Position = new CameraSpacePoint();
                //j.Position.X = 50;
                //j.Position.Y = 100;
                //j.TrackingState = TrackingState.Tracked;
                //joints.Add(j);


                //Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                //jointPoints.Add(JointType.AnkleLeft, new Point(1,1));
                //jointPoints.Add(JointType.AnkleRight, new Point(1, 2));

                //jointPoints.Add(JointType.ElbowLeft, new Point(1, 1));
                //jointPoints.Add(JointType.ElbowRight, new Point(1, 2));

                //jointPoints.Add(JointType.FootLeft, new Point(2, 1));
                //jointPoints.Add(JointType.FootRight, new Point(2, 2));

                //SendJoinPoints(joints);

            }


    
        }

        private static void SendJoinPoints(List<Joint>  joints)
        {

            if (joints.Count > 0)//podría ser bodies
            {
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(joints);


                foreach (var socket in _clients)
                {
                    socket.Send(json);
                }
            }
        }


        private static void SendJoinPoints(Dictionary<JointType, Point> joints)
        {

            if (joints.Count > 0)//podría ser bodies
            {
                //var serializer = new JavaScriptSerializer();
                //var json = serializer.Serialize(joints);


                //IDictionary<string, int> dict = new Dictionary<string, int>();
                //dict.Add("some key", 1);
                //dict.Add("another key", 5);

                var json = SerializeJointDictionary(joints);
                Console.SetCursorPosition(0,15);
                Console.WriteLine("Head: " + joints[JointType.Head].X + "" + joints[JointType.Head].Y.ToString());

                Console.WriteLine("RightHand:   X: " + joints[JointType.HandRight].X + " Y: " + joints[JointType.HandRight].Y.ToString());
                Console.WriteLine("LeftHand:  X: " + joints[JointType.HandLeft].X + " Y: " + joints[JointType.HandLeft].Y.ToString());

                foreach (var socket in _clients)
                {
                    socket.Send(json);
                }
            }
        }

        private static string SerializeJointDictionary(IDictionary<JointType, Point> dict)
        {
            string json = JsonConvert.SerializeObject(dict, new CustomDictionaryConverter());
            //Console.WriteLine(json);
            return json;
        }


        private static void GiveMeBodies()
        {

            //mockear bodies?
           // bodies[0] = new Body()
            //throw new NotImplementedException();
        }




        //static void Sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        //{
        //    if (!_serverInitialized) return;

           



        //    // Create the drawing group we'll use for drawing
        //    drawingGroup = new DrawingGroup();

        //    // Create an image source that we can use in our image control
        //    imageSource = new DrawingImage(drawingGroup);

        //    // use the window object as the view model in this simple example
        //    DataContext = this;

        //    // initialize the components (controls) of the window
        //    InitializeComponent();



        //    List<Skeleton> users = new List<Skeleton>();

        //    using (var frame = e.OpenSkeletonFrame())
        //    {
        //        if (frame != null)
        //        {
        //            frame.CopySkeletonDataTo(_skeletons);

        //            foreach (var skeleton in _skeletons)
        //            {
        //                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
        //                {
        //                    users.Add(skeleton);
        //                }
        //            }

        //            if (users.Count > 0)
        //            {
        //                string json = users.Serialize();

        //                foreach (var socket in _clients)
        //                {
        //                    socket.Send(json);
        //                }
        //            }
        //        }
        //    }
        //}


        /// <summary>
        /// Handles the body frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        static private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            BodyFrameReference frameReference = e.FrameReference;
            

            if (_startTime == 0)
            {
                _startTime = frameReference.RelativeTime;
            }

            try
            {
                BodyFrame frame = frameReference.AcquireFrame();

                if (frame != null)
                {
                    // BodyFrame is IDisposable
                    using (frame)
                    {
                        framesSinceUpdate++;

                        // update status unless last message is sticky for a while
                        if (DateTime.Now >= _nextStatusUpdate)
                        {
                            // calcuate fps based on last frame received
                            double fps = 0.0;

                            if (stopwatch.IsRunning)
                            {
                                stopwatch.Stop();
                                fps = framesSinceUpdate / stopwatch.Elapsed.TotalSeconds;
                                stopwatch.Reset();
                            }

                            _nextStatusUpdate = DateTime.Now + TimeSpan.FromSeconds(1);
                            var StatusText = string.Format("FPS = {0:N1} Time = {1}", fps, frameReference.RelativeTime - _startTime);
                            
                            Console.SetCursorPosition(0,5);
                            Console.WriteLine(StatusText);
                        }

                        if (!stopwatch.IsRunning)
                        {
                            framesSinceUpdate = 0;
                            stopwatch.Start();
                        }


                        



                        // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                        // As long as those body objects are not disposed and not set to null in the array,
                        // those body objects will be re-used.
                        frame.GetAndRefreshBodyData(bodies);

                        int sentbodies = 0;
                        foreach (Body body in bodies)
                        {
                            if (body.IsTracked && sentbodies ==0)
                            {
                                //DrawClippedEdges(body, dc);

                                IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                                // convert the joint points to depth (display) space
                                Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();
                                foreach (JointType jointType in joints.Keys)
                                {
                                    DepthSpacePoint depthSpacePoint = _coordinateMapper.MapCameraPointToDepthSpace(joints[jointType].Position);
                                    jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                                }

                                //ahora dibujar...!!!

                                //tu metelo ahí, que ya veremos como lo dibujas.
                                // convert the joint points to depth (display) space
                                //mockeamos un cuerpo un poco deforme
                                //List<Joint> joints = new List<Joint>();

                                //this.DrawBody(joints, jointPoints, dc);

                                //this.DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                                //this.DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);

                                SendJoinPoints(jointPoints);
                                sentbodies++; //manda solo un body, que nos podemos volver locos...

                                // SendJoinPoints(joints);
                            }
                        }




                        //using (DrawingContext dc = this.drawingGroup.Open())
                        //{
                        //    // Draw a transparent background to set the render size
                        //    dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));

                        //    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                        //    // As long as those body objects are not disposed and not set to null in the array,
                        //    // those body objects will be re-used.
                        //    frame.GetAndRefreshBodyData(this.bodies);

                        //    foreach (Body body in this.bodies)
                        //    {
                        //        if (body.IsTracked)
                        //        {
                        //            this.DrawClippedEdges(body, dc);

                        //            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                        //            // convert the joint points to depth (display) space
                        //            Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();
                        //            foreach (JointType jointType in joints.Keys)
                        //            {
                        //                DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(joints[jointType].Position);
                        //                jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                        //            }

                        //            this.DrawBody(joints, jointPoints, dc);

                        //            this.DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                        //            this.DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);
                        //        }
                        //    }

                        //    // prevent drawing outside of our render area
                        //    this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                        //}
                    }
                }
            }
            catch (Exception)
            {
                // ignore if the frame is no longer available
            }
        }
    }
}
