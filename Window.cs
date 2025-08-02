using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComputerGraphicsProgramming
{
    public partial class Window : Form
    {
        private MainMenu mainMenu;
        private bool createShapeStatus = false;
        private bool createRotatePointStatus = false;
        private bool rotationPointStatus = false;
        private bool createMirrorLineStatus = false;
        private bool mirrorLineStatus = false;
        private int movingPointIndex = -1;
        private int shapeSelected = -1;
        private int clicknumber = 0;
        private int targetclicks = 0;
        private Point[] selectedPoints;
        private Circle rotationPoint;
        private Line mirrorLine;
        private List<(Shape shape, Pen pen, bool selected)> ShapesData = new List<(Shape, Pen, bool)>();
        private List<(Shape shape, Pen pen)> MirrorShapesData = new List<(Shape, Pen)>();
        private Shape newShape;
        private DialForm dialForm;

        public Window()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;
            this.DoubleBuffered = true;

            //Main Menu
            MainMenu mainMenu = new MainMenu();

            //Create
            MenuItem createItem = new MenuItem();
            MenuItem createPolygonItem = new MenuItem();
            MenuItem createSquareItem = new MenuItem();
            MenuItem createTriangleItem = new MenuItem();
            MenuItem createCircleItem = new MenuItem();
            createItem.Text = "Create";
            createPolygonItem.Text = "Polygon";
            createSquareItem.Text = "Square";
            createTriangleItem.Text = "Triangle";
            createCircleItem.Text = "Circle";
            mainMenu.MenuItems.Add(createItem);
            createItem.MenuItems.Add(createPolygonItem);
            createItem.MenuItems.Add(createSquareItem);
            createItem.MenuItems.Add(createTriangleItem);
            createItem.MenuItems.Add(createCircleItem);

            //Roation
            MenuItem rotationItem = new MenuItem();
            rotationItem.Text = "&Rotation";
            mainMenu.MenuItems.Add(rotationItem);

            //Rotation - Rotation Dial
            MenuItem rotationDialItem = new MenuItem();
            rotationDialItem.Text = "Open Rotation Dial";
            rotationItem.MenuItems.Add(rotationDialItem);

            //Rotation - Rotate Point
            MenuItem createRotatePointItem = new MenuItem();
            MenuItem deleteRotatePointItem = new MenuItem();
            createRotatePointItem.Text = "Create Rotate Point";
            deleteRotatePointItem.Text = "Delete Rotate Point";
            rotationItem.MenuItems.Add(createRotatePointItem);
            rotationItem.MenuItems.Add(deleteRotatePointItem);

            //Scaling
            MenuItem scalingItem = new MenuItem();
            scalingItem.Text = "Scaling";
            mainMenu.MenuItems.Add(scalingItem);

            //Scaling - Scale Up and Down
            MenuItem scaleUpItem = new MenuItem();
            MenuItem scaleDownItem = new MenuItem();
            scaleUpItem.Text = "Scale Up Shapes";
            scaleDownItem.Text = "Scale Down Shapes";
            scalingItem.MenuItems.Add(scaleUpItem);
            scalingItem.MenuItems.Add(scaleDownItem);

            //Mirroring
            MenuItem mirroringItem = new MenuItem();
            mirroringItem.Text = "&Mirroring";
            mainMenu.MenuItems.Add(mirroringItem);

            //Mirroring - Mirror Line
            MenuItem createMirrorLine = new MenuItem();
            MenuItem deleteMirrorLine = new MenuItem();
            createMirrorLine.Text = "Create Mirror Line";
            deleteMirrorLine.Text = "Delete Mirror Line";
            mirroringItem.MenuItems.Add(createMirrorLine);
            mirroringItem.MenuItems.Add(deleteMirrorLine);

            //Delete
            MenuItem deleteItem = new MenuItem();
            deleteItem.Text = "Delete";
            mainMenu.MenuItems.Add(deleteItem);

            //Delete - All Shapes or Selected Shapes
            MenuItem deleteSelectedItem = new MenuItem();
            MenuItem deleteAllItem = new MenuItem();
            deleteSelectedItem.Text = "Delete Selected Shapes";
            deleteAllItem.Text = "Delete All Shapes";
            deleteItem.MenuItems.Add(deleteSelectedItem);
            deleteItem.MenuItems.Add(deleteAllItem);

            //Menu Item Click Events
            createPolygonItem.Click += new System.EventHandler(CreatePolygon);
            createSquareItem.Click += new System.EventHandler(CreateSquare);
            createTriangleItem.Click += new System.EventHandler(CreateTriangle);
            createCircleItem.Click += new System.EventHandler(CreateCircle);
            rotationDialItem.Click += new System.EventHandler(RotationDial);
            createRotatePointItem.Click += new System.EventHandler(CreateRotatePoint);
            createMirrorLine.Click += new System.EventHandler(CreateMirrorLine);


            deleteRotatePointItem.Click += (sender, e) => DeleteRotatePoint();
            deleteMirrorLine.Click += (sender, e) => DeleteMirrorLine();
            scaleUpItem.Click += (sender, e) => ScaleShapes(1.1f);
            scaleDownItem.Click += (sender, e) => ScaleShapes(0.9f);
            deleteSelectedItem.Click += (sender, e) => DeleteShapes(true);
            deleteAllItem.Click += (sender, e) => DeleteShapes(false);

            this.Paint += DrawAll;
            this.Menu = mainMenu;
            this.MouseClick += MouseClicked;
            this.MouseMove += MouseMoved;
            this.MouseUp += MouseReleased;
        }
        private void ResetStatus()
        {
            //Reset all statuses to neutral 
            clicknumber = 0;
            selectedPoints = null;
            createShapeStatus = false;
            createRotatePointStatus = false;
            createMirrorLineStatus = false;
            newShape = null;
            this.Invalidate();
        }
        private void CreatePolygon(object sender, EventArgs e)
        {
            //Create Shape Polygon object
            ResetStatus();
            createShapeStatus = true;
            targetclicks = 4;
            newShape = new Polygon();
            MessageBox.Show("Click once each at four locations to create a polygon");
        }
        private void CreateSquare(object sender, EventArgs e)
        {
            //Create Shape Square object
            ResetStatus();
            createShapeStatus = true;
            targetclicks = 2;
            newShape = new Square();
            MessageBox.Show("Click once each at two locations to create a square");
        }
        private void CreateTriangle(object sender, EventArgs e)
        {
            //Create Shape Triangle object
            ResetStatus();
            createShapeStatus = true;
            targetclicks = 3;
            newShape = new Triangle();
            MessageBox.Show("Click once each at three locations to create a triangle");
        }
        private void CreateCircle(object sender, EventArgs e)
        {
            //Create Shape Circle object
            ResetStatus();
            createShapeStatus = true;
            targetclicks = 2;
            newShape = new Circle();
            MessageBox.Show("Click once each at two locations to create a circle");
        }
        private void DeleteShapes(bool selectedOnly)
        {
            //Delete selected shapes
            if (selectedOnly)
            {
                ShapesData.RemoveAll(item => item.selected);
            }
            //Delete all shapes
            else
            {
                ShapesData.Clear();
            }
            this.Invalidate();
        }
        private void ScaleShapes(float scaleFactor)
        {
            //Iterate through shape data list
            for (int i = 0; i < ShapesData.Count; i++)
            {
                //Check shape selcted
                if (ShapesData[i].selected == true)
                {
                    //Scale shape with scaling matrix and set new shape points
                    Matrix2D scalingMatrix = new Matrix2D(scaleFactor, ShapesData[i].shape.GetCentrePoint());
                    PointF[] shapePoints = ShapesData[i].shape.GetPoints();
                    shapePoints = Matrix2D.TransformMultiplePoints(scalingMatrix, shapePoints);
                    ShapesData[i].shape.SetPoints(shapePoints);
                }
            }
            //Redraw
            this.Invalidate();
        }
        private void CreateRotatePoint(object sender, EventArgs e)
        {
            //Create a rotation point
            ResetStatus();
            createRotatePointStatus = true;
            MessageBox.Show("Click to set Rotation Point");
        }
        private void DeleteRotatePoint()
        {
            //Reset rotation point attributes
            rotationPointStatus = false;
            rotationPoint = null;
            this.Invalidate();
        }
        private void RotationDial(object sender, EventArgs e)
        {
            //Show the dialForm to get user input for rotation angle
            if (dialForm == null || dialForm.IsDisposed) //Prevent multiple instances
            {
                dialForm = new DialForm();
                //When rotate angle is updated rotate shapes
                dialForm.RotateAngle += rotateShapes;
                dialForm.Show();
            }
            else
            {
                dialForm.BringToFront();
            }
        }
        private void rotateShapes(int angle)
        {
            //Create transformation Matrix for rotating with no translation
            Matrix2D transformationMatrix = new Matrix2D(angle, new PointF(0, 0), new PointF(0, 0));
            //Iterate through shapes data list
            for (int i = 0; i < ShapesData.Count; i++)
            {
                //Check shape selected
                if (ShapesData[i].selected == true)
                {
                    //Check if rotation point calculation needs to be used
                    if (rotationPointStatus)
                    {
                        PointF[] shapePoints = ShapesData[i].shape.GetPoints();
                        shapePoints = Matrix2D.TransformMultiplePoints(transformationMatrix, shapePoints, rotationPoint.GetCentrePoint());
                        ShapesData[i].shape.SetPoints(shapePoints);
                    }
                    //If no rotation point is created rotate shape at shape centre
                    else
                    {
                        PointF shapeCentre = ShapesData[i].shape.GetCentrePoint();
                        PointF[] shapePoints = ShapesData[i].shape.GetPoints();
                        shapePoints = Matrix2D.TransformMultiplePoints(transformationMatrix, shapePoints, shapeCentre);
                        ShapesData[i].shape.SetPoints(shapePoints);
                    }
                }
            }
            //Redraw
            this.Invalidate();
        }
        private void CreateMirrorLine(object sender, EventArgs e)
        {
            //Delete any previous mirror line and enable creation of a mirror line 
            ResetStatus();
            DeleteMirrorLine();
            createMirrorLineStatus = true;
            mirrorLine = new Line();
            MessageBox.Show("Click to set Mirror Line");
        }
        private void DeleteMirrorLine()
        {
            //Reset mirror line attributes to neutral / null
            mirrorLineStatus = false;
            MirrorShapesData = null;
            mirrorLine = null;
            //Redraw
            this.Invalidate();
        }
        private void DrawAll(object sender, PaintEventArgs e)
        {
            // Create graphics objects
            Graphics g = e.Graphics;

            //Draw new shape (shape being created) if not null
            if (newShape != null)
            {
                newShape.DrawShape(g, new Pen(Color.Black));
            }
            //Draw all stored shapes
            for (int i = 0; i < ShapesData.Count; i++)
            {
                ShapesData[i].shape.DrawShape(g, ShapesData[i].pen);

                //If the shape is selected annotate shape points
                if (ShapesData[i].selected == true)
                {
                    ShapesData[i].shape.AnnotatePoints(g, new Pen(Color.Black));
                }
            }
            //If Rotate Point created draw Rotate Point
            if (rotationPointStatus)
            {
                rotationPoint.DrawShape(g, new Pen(Color.Blue));
            }
            //Draw mirror line
            if (mirrorLine != null)
            {
                mirrorLine.DrawLine(g, new Pen(Color.Blue));
            }
            //If mirror line is complete (mirrorLineStatus == true)
            if (mirrorLineStatus == true)
            {
                Point[] mirrorLinePoints = mirrorLine.GetPoints();
                //Initialise MirrorShapesData and mirrorMatrix
                MirrorShapesData = new List<(Shape, Pen)>();
                Matrix2D mirrorMatrix = new Matrix2D(mirrorLinePoints);

                //Iterate through shapes data list
                for (int i = 0; i < ShapesData.Count; i++)
                {
                    //Clone shape to mirror shapes data
                    MirrorShapesData.Add((ShapesData[i].shape.Clone(), new Pen(Color.Blue)));
                    //Calculate and set reflected shape points as new shape points
                    MirrorShapesData[i].shape.SetPoints(Matrix2D.TransformMultiplePoints(mirrorMatrix, MirrorShapesData[i].shape.GetPoints(), mirrorLinePoints[0]));
                    //Draw reflected shape
                    MirrorShapesData[i].shape.DrawShape(g, MirrorShapesData[i].pen);
                }
            }
        }
        private void MouseMoved(object sender, MouseEventArgs e)
        {
            //Select the correct function to call
            if (createShapeStatus && selectedPoints != null)
            {
                UpdateShapePreview(e.Location);
                return;
            }
            if (createMirrorLineStatus && selectedPoints != null)
            {
                UpdateMirrorLinePreview(e.Location);
                return;
            }
            if (e.Button == MouseButtons.Left)
            {
                HandleShapeDragging(e.Location);
            }
        }
        private void UpdateShapePreview(Point mouseLocation)
        {
            //For selected points of shape without user input value
            for (int i = clicknumber; i < selectedPoints.Length; i++)
            {
                //Set point to mouse location
                selectedPoints[i] = mouseLocation;
            }
            //Construct shape with unset points at mouse location
            newShape.ConstructShape(selectedPoints);
            //Redraw
            this.Invalidate();  
        }
        private void UpdateMirrorLinePreview(Point mouseLocation)
        {            
            //Set mirror line unset point to mouse location
            mirrorLine.SetPoints(selectedPoints[0], mouseLocation);
            //Redraw
            this.Invalidate();      
        }
        private void HandleShapeDragging(Point mouseLocation)
        {
            //Iterate through shapes data list
            for (int i = 0; i < ShapesData.Count; i++)
            {
                //Find selected shapes
                if (ShapesData[i].selected == true)
                {
                    //If shape point index is not set i.e -1 find shape point
                    if (movingPointIndex == -1)
                    {
                        //Retrieve shape point index in range of mouse location
                        movingPointIndex = ShapesData[i].shape.GetShapePointInRange(mouseLocation);
                        shapeSelected = i;
                    }
                    //If shape point index already selected continue to update position
                    else if (movingPointIndex >= 0)
                    {
                        PointF[] shapePoints = ShapesData[shapeSelected].shape.GetPoints();

                        //Set new shape point of selected point to mouse position
                        shapePoints[movingPointIndex] = mouseLocation;

                        //Update stored Shape Points of the object
                        ShapesData[shapeSelected].shape.SetPoints(shapePoints);
                        //Redraw
                        this.Invalidate();
                    }
                    //If shape point index already selected continue to update position
                    else if (movingPointIndex == -2)
                    {
                        PointF shapeCentre = ShapesData[shapeSelected].shape.GetCentrePoint();
                        //Initialise translation matrix of shape centre point to mouse point
                        Matrix2D translationMatrix = new Matrix2D(mouseLocation, shapeCentre);
                        //Apply translation matrix to all shape points and set new shape points
                        ShapesData[shapeSelected].shape.SetPoints(Matrix2D.TransformMultiplePoints(translationMatrix, ShapesData[shapeSelected].shape.GetPoints()));
                        //Redraw
                        this.Invalidate();
                    }
                }
            }            
        }
        private void MouseReleased(object sender, MouseEventArgs e)
        {
            //Reset statuses related to point selection, de-select any selected point
            movingPointIndex = -1;
            shapeSelected = -1;
        }
        private void MouseClicked(object sender, MouseEventArgs e)
        {
            //Check if mouse click is not a left click
            if (e.Button != MouseButtons.Left)
                return;
            //Select appropriate function call to status
            if (createShapeStatus)
            {
                HandleShapeCreation(e.Location);
                return;
            }
            if (createRotatePointStatus)
            {
                HandleRotationPoint(e.Location);
                return;
            }
            if (createMirrorLineStatus)
            {
                HandleMirrorLine(e.Location);
                return;
            }
            if (movingPointIndex == -1)
            {
                HandleShapeSelection(e.Location);
                return;
            }
        }
        private void HandleShapeCreation(Point mouseLocation)
        {
            //Check if click number is less than target click number
            if (clicknumber < targetclicks)
            {
                if (clicknumber == 0)
                {
                    //Initialise SelectedPoints array
                    selectedPoints = new Point[targetclicks];
                }
                //Store mouse click position in clicknumber index
                selectedPoints[clicknumber] = mouseLocation;
                clicknumber++;
            }
            //Check if click number has reached target click number
            if (clicknumber == targetclicks)
            {
                //Construct the new shape and add to ShapesData list with default variables
                newShape.ConstructShape(selectedPoints);
                ShapesData.Add((newShape, new Pen(Color.Black), false));
                ResetStatus();
            }
            
        }
        private void HandleRotationPoint(Point mouseLocation)
        {
            //Rotation point requires 1 click only
            if (clicknumber < 1)
            {
                //Initialise and construct rotation point circle at mouse click point
                rotationPoint = new Circle();
                rotationPoint.ConstructShape(new Point[]
                {
                        new Point(mouseLocation.X -3, mouseLocation.Y -3),
                        new Point(mouseLocation.X +3, mouseLocation.Y +3)
                });
                //Update statuses
                clicknumber++;
                createRotatePointStatus = false;
                rotationPointStatus = true;
                //Redraw
                this.Invalidate();
            }
        }
        private void HandleMirrorLine(Point mouseLocation)
        {
            //Mirror Line requires 2 clicks to create
            if (clicknumber < 2)
            {
                //Initialise selectedPoints array
                if (clicknumber == 0)
                {
                    selectedPoints = new Point[2];
                }
                //Update stored points
                selectedPoints[clicknumber] = mouseLocation;
                clicknumber++;
            }
            //Check if click target has been reached
            if (clicknumber == 2)
            {
                //Update statuses
                createMirrorLineStatus = false;
                mirrorLineStatus = true;
                mirrorLine = new Line(selectedPoints[0], selectedPoints[1]);
                //Redraw
                this.Invalidate();
            }
        }
        private void HandleShapeSelection(Point mouseLocation)
        {
            //Iterate through shapes data list
            for (int i = 0; i < ShapesData.Count; i++)
            {
                //Check if mouse location point is inside of shape
                if (ShapesData[i].shape.IsPointInShape(mouseLocation))
                {
                    //If shape already selected then de-select
                    if (ShapesData[i].selected == true)
                    {
                        ShapesData[i] = (ShapesData[i].shape, new Pen(Color.Black), false);
                    }
                    //Else Select shape
                    else
                    {
                        ShapesData[i] = (ShapesData[i].shape, new Pen(Color.Red), true);
                    }
                    //Redraw
                    this.Invalidate();
                }
            }
        }
    }
    public class DialForm : Form
    {
        private Label lblAngle;
        private Button btnRotate;
        private Button btnToggleMode;
        private int angle = 0;
        private int calculatedAngle = 0;
        private bool clockwiseMode = true;

        //Event to notify the main form
        public event Action<int> RotateAngle;

        public DialForm()
        {
            this.Text = "Select Angle";
            this.Size = new Size(300, 350);
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterParent;

            //Create lables and buttons
            lblAngle = new Label
            {
                Text = "Angle: 0°",
                Location = new Point(125, 230),
                AutoSize = true
            };
            btnRotate = new Button
            {
                Text = "Rotate",
                Location = new Point(110, 280)
            };
            btnToggleMode = new Button
            {
                Text = "Anti-Clockwise",
                Location = new Point(90, 250),
                Size = new Size(120, 30)
            };
            this.Controls.Add(lblAngle);
            this.Controls.Add(btnRotate);
            this.Controls.Add(btnToggleMode);

            //On click events
            btnRotate.Click += (s, e) => { RotateAngle?.Invoke(calculatedAngle); };
            btnToggleMode.Click += ToggleAngleMode;

            //Draw Dial On Paint
            this.Paint += DrawDial;

            //Update rotation with mouse movement
            this.MouseMove += RotateDial;
        }
        private void DrawDial(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            //Place centre
            int centreX = 150;
            int centreY = 110;
            int radius = 80;
            int circleRadius = 56;

            //Create and draw Circle
            Circle circle = new Circle();
            circle.ConstructShape(new Point[] 
            { 
                new Point(centreX -circleRadius, centreY -circleRadius), 
                new Point(centreX + circleRadius, centreY + circleRadius) 
            });
            circle.DrawShape(g, new Pen(Color.Black));

            //Calculate hand position using absolute of angle to avoid negative values
            double radians = Math.Abs(angle) * (Math.PI / 180);
            int handX = centreX + (int)(radius * Math.Cos(radians - (Math.PI / 2)));
            int handY = centreY + (int)(radius * Math.Sin(radians - (Math.PI / 2)));

            //Draw angle selection hand
            Line line = new Line(new Point(centreX, centreY), new Point(handX, handY));
            line.DrawLine(g, new Pen(Color.Red));
        }
        private void RotateDial(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int centerX = 150;
                int centerY = 110;

                //Calculate angle from centre of placed circle to mouse position
                double radians = Math.Atan2(e.Y - centerY, e.X - centerX);
                angle = (int)(((radians * 180) / Math.PI) +90);//Add 90 to angle to align with hand

                //Normalise angle to be within 0-360 range
                if (angle < 0)
                {
                    angle += 360;
                }

                //Update angle and Redraw
                UpdateAngle();
                this.Invalidate();
            }
        }
        private void ToggleAngleMode(object sender, EventArgs e)
        {
            //Switch the rotation mode bool
            clockwiseMode = !clockwiseMode;

            //Update button text
            btnToggleMode.Text = clockwiseMode ? "Anti-Clockwise" : "Clockwise";

            UpdateAngle();
        }
        private void UpdateAngle()
        {
            //Check clockwiseMode bool and invert angle accordingly
            calculatedAngle = clockwiseMode ? angle : - angle;
            //Update lable text
            lblAngle.Text = $"Angle: {calculatedAngle:0}°";
        }
    }
    public class Matrix2D
    {
        public double[,] matrix;
        public Matrix2D(int angle, PointF newCentre, PointF oldCentre)
        {
            //2D Transformation matrix for rotation and translation

            //Calculate translation offset
            double dX = newCentre.X - oldCentre.X;
            double dY = newCentre.Y - oldCentre.Y;

            //Convert degrees to radians
            double radians = angle * (Math.PI / 180.0);

            //Compute Sin and Cosin
            double sinA = Math.Sin(radians);
            double cosA = Math.Cos(radians);
            //Store as matrix
            matrix = new double[,] 
            { 
                { cosA, -sinA, dX }, 
                { sinA, cosA, dY }, 
                { 0, 0, 1 } 
            };
        }
        public Matrix2D(PointF newCentre, PointF oldCentre)
        {
            //2D Translation matrix

            //Calculate translation offset
            double dX = newCentre.X - oldCentre.X;
            double dY = newCentre.Y - oldCentre.Y;

            matrix = new double[,] 
            { 
                { 1, 0, dX},
                { 0, 1, dY},
                { 0, 0, 1} 
            };
        }
        public Matrix2D(float scaleFactor, PointF centre)
        {
            //2D Scaling matrix

            matrix = new double[,] 
            {
                { scaleFactor, 0, (1 - scaleFactor) * centre.X },
                { 0, scaleFactor, (1 - scaleFactor) * centre.Y },
                { 0, 0, 1 }
            };
        }
        public Matrix2D(Point[] mirrorLine) 
        {
            //2D Mirroring matrix

            //Convert mirror line to direction vector
            float dx = mirrorLine[1].X - mirrorLine[0].X;
            float dy = mirrorLine[1].Y - mirrorLine[0].Y;

            //Normalise direction vector
            float length = (float)Math.Sqrt(dx * dx + dy * dy);
            float ux = dx / length;
            float uy = dy / length;

            matrix = new double[,]
            {
                {(2* ux * ux -1), (2 * ux * uy), 0},
                {(2* ux * uy), (2 * uy * uy - 1), 0},
                {0, 0, 1}
            };
        }
        public Matrix2D(double[,] toSet)
        {
            //Create Matrix2D object from 2D array

            matrix = toSet;
        }
        public int NumberOfColumns()
        {
            return matrix.GetLength(1);
        }
        public int NumberOfRows()
        {
            return matrix.GetLength(0);
        }
        public static PointF TransformPoint(Matrix2D MatrixA, PointF point, PointF pivot)
        {
            //Create point Matrix for multiplication
            Matrix2D MatrixB = new Matrix2D(new double[,] 
            { 
                { point.X -pivot.X }, 
                { point.Y - pivot.Y}, 
                { 1 } 
            }); //Account for any Pivot point

            double temp;
            int RowsA = MatrixA.NumberOfRows();
            int ColsA = MatrixA.NumberOfColumns();
            int ColsB = MatrixB.NumberOfColumns();
            float[,] newPoint = new float[RowsA, ColsB];

            //Loop through all rows in matrix A
            for (int i = 0; i < RowsA; i++)
            {
                //Loop through all columns in matrix B (for each row of matrix A)
                for (int j = 0; j < ColsB; j++)
                {
                    temp = 0;
                    //Loop through all indexes in row/column
                    for (int k = 0; k < ColsA; k++)
                    {
                        //Perform multiplication for all indices of row and column and add to temp result
                        temp += MatrixA.matrix[i, k] * MatrixB.matrix[k, j];
                    }
                    //Save result
                    newPoint[i, j] = (float)temp;
                }
            }
            //Return new point 
            return new PointF(newPoint[0,0] + pivot.X, newPoint[1,0] + pivot.Y);
        }
        public static PointF TransformPoint(Matrix2D MatrixA, PointF point)
        {
            //Redundancy for pivot point
            return TransformPoint(MatrixA, point, new Point(0, 0));
        }
        public static PointF[] TransformMultiplePoints(Matrix2D MatrixA, PointF[] shapePoints, PointF pivot)
        {
            PointF[] newPoints = new PointF[shapePoints.Length];
            for (int i = 0; i < shapePoints.Length; i++)
            {
                newPoints[i] = TransformPoint(MatrixA, shapePoints[i], pivot);
            }
            return newPoints;
        }
        public static PointF[] TransformMultiplePoints(Matrix2D MatrixA, PointF[] shapePoints)
        {
            //Redundancy for pivot point
            return TransformMultiplePoints(MatrixA, shapePoints, new PointF(0, 0));
        }
    }
    public abstract class Shape
    {
        //Abstract class Shape

        protected PointF[] shapePoints;
        public Shape()
        { 
        }
        public abstract void DrawShape(Graphics g, Pen pen);
        public abstract void ConstructShape(Point[] selectedPoints);
        public PointF[] GetPoints()
        {
            return shapePoints;
        }
        public void SetPoints(PointF[] points)
        {
            shapePoints = points;
        }
        public PointF GetCentrePoint()
        {
            float centreX = 0f;
            float centreY = 0f;

            for (int i = 0; i < shapePoints.Length; i++)
            {
                centreX += shapePoints[i].X;
                centreY += shapePoints[i].Y;
            }
            centreX = centreX / shapePoints.Length;
            centreY = centreY / shapePoints.Length;

            return new PointF(centreX, centreY);
        }
        public void AnnotatePoints(Graphics g, Pen pen)
        {
            Circle annotationCircle = new Circle();
            annotationCircle.ConstructShape(new Point[] { new Point(0, 0), new Point(10, 10) });
            for(int i = 0; i < shapePoints.Length; i++)
            {
                Matrix2D translationMatrix = new Matrix2D(shapePoints[i], annotationCircle.GetCentrePoint());
                annotationCircle.SetPoints(Matrix2D.TransformMultiplePoints(translationMatrix, annotationCircle.GetPoints()));
                annotationCircle.DrawShape(g, pen);
            }
            Matrix2D translationMatrix2 = new Matrix2D(GetCentrePoint(), annotationCircle.GetCentrePoint());
            annotationCircle.SetPoints(Matrix2D.TransformMultiplePoints(translationMatrix2, annotationCircle.GetPoints()));
            annotationCircle.DrawShape(g, pen);
        }
        public int GetShapePointInRange(PointF point)
        {
            Circle detectionCircle = new Circle();
            detectionCircle.ConstructShape(new Point[] { new Point(0, 0), new Point(10, 10) });
            for (int i = 0; i < shapePoints.Length; i++)
            {
                Matrix2D translationMatrix = new Matrix2D(0, shapePoints[i], detectionCircle.GetCentrePoint());
                detectionCircle.SetPoints(Matrix2D.TransformMultiplePoints(translationMatrix, detectionCircle.GetPoints()));
                if (detectionCircle.IsPointInShape(point) == true)
                {
                    return i;
                }
            }
            Matrix2D translationMatrix2 = new Matrix2D(0, GetCentrePoint(), detectionCircle.GetCentrePoint());
            detectionCircle.SetPoints(Matrix2D.TransformMultiplePoints(translationMatrix2, detectionCircle.GetPoints()));
            if (detectionCircle.IsPointInShape(point) == true)
            {
                return -2;
            }
            return -1;
        }
        public abstract bool IsPointInShape(PointF point);
        public abstract Shape Clone();
    }
    class Square : Shape
    {
        public Square()
        {
            shapePoints = new PointF[4];
        }
        public override void ConstructShape(Point[] selectedPoints)
        {
            //Calculate side length (max difference in X or Y)
            float sideLength = Math.Max(Math.Abs(selectedPoints[1].X - selectedPoints[0].X), Math.Abs(selectedPoints[1].Y - selectedPoints[0].Y));

            //Determine the top-left point
            float minX = Math.Min(selectedPoints[0].X, selectedPoints[1].X);
            float minY = Math.Min(selectedPoints[0].Y, selectedPoints[1].Y);
            PointF topLeft = new PointF(minX, minY);

            shapePoints[0] = topLeft; //Top-left
            shapePoints[1] = new PointF(topLeft.X + sideLength, topLeft.Y); //Top-right
            shapePoints[2] = new PointF(topLeft.X + sideLength, topLeft.Y + sideLength); //Bottom-right
            shapePoints[3] = new PointF(topLeft.X, topLeft.Y + sideLength);  //Bottom-left
        }
        public override void DrawShape(Graphics g, Pen pen)
        {
            Line line = new Line();
            //Draw line between all points except last to first
            for (int i = 0; i <= shapePoints.Length - 2; i++)
            {
                line.SetPoints(shapePoints[i], shapePoints[i + 1]);
                line.DrawLine(g, pen);
            }
            line.SetPoints(shapePoints[shapePoints.Length - 1], shapePoints[0]);
            line.DrawLine(g, pen);
        }
        public override bool IsPointInShape(PointF point)
        {
            bool inside = false;
            int n = shapePoints.Length;

            //Check using Ray-Casting method
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                if (((shapePoints[i].Y > point.Y) != (shapePoints[j].Y > point.Y)) &&
                    (point.X < (shapePoints[j].X - shapePoints[i].X) *
                    (point.Y - shapePoints[i].Y) / (shapePoints[j].Y - shapePoints[i].Y) + shapePoints[i].X))
                {
                    inside = !inside;
                }
            }

            return inside;
        }
        public override Shape Clone()
        {
            //Return new Square object of shapePoints
            var clone = new Square();
            clone.SetPoints(shapePoints);
            return clone;
        }
    }
    class Polygon : Shape
    {
        public Polygon()
        {
            shapePoints = new PointF[4];
        }
        public override void ConstructShape(Point[] SelectedPoints)
        {
            //Shape Points are equal to Selected Points
            for (int i = 0; i < SelectedPoints.Length; i++)
            {
                shapePoints[i] = SelectedPoints[i];
            }
        }
        public override void DrawShape(Graphics g, Pen pen)
        {
            Line line = new Line();
            //Draw line between all points except last to first
            for (int i = 0; i <= shapePoints.Length - 2; i++)
            {
                line.SetPoints(shapePoints[i], shapePoints[i + 1]);
                line.DrawLine(g, pen);
            }
            line.SetPoints(shapePoints[shapePoints.Length - 1], shapePoints[0]);
            line.DrawLine(g, pen);
        }
        public override bool IsPointInShape(PointF point)
        {
            bool inside = false;
            int n = shapePoints.Length;

            //Check using Ray-Casting method
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                if (((shapePoints[i].Y > point.Y) != (shapePoints[j].Y > point.Y)) &&
                    (point.X < (shapePoints[j].X - shapePoints[i].X) *
                    (point.Y - shapePoints[i].Y) / (shapePoints[j].Y - shapePoints[i].Y) + shapePoints[i].X))
                {
                    inside = !inside;
                }
            }

            return inside;
        }
        public override Shape Clone()
        {
            //Return new Polygon object of shapePoints
            var clone = new Polygon();
            clone.SetPoints(shapePoints);
            return clone;
        }
    }
    class Triangle : Shape
    {
        public Triangle()
        {
            shapePoints = new PointF[3];
        }
        public override void ConstructShape(Point[] SelectedPoints)
        {
            //Shape Points are equal to Selected Points
            for (int i = 0; i < SelectedPoints.Length; i++)
            {
                shapePoints[i] = SelectedPoints[i];
            }
        }
        public override void DrawShape(Graphics g, Pen pen)
        {
            Line line = new Line();
            //Draw line between all points except last to first
            for (int i = 0; i <= shapePoints.Length - 2; i++)
            {
                line.SetPoints(shapePoints[i], shapePoints[i + 1]);
                line.DrawLine(g, pen);
            }
            line.SetPoints(shapePoints[shapePoints.Length - 1], shapePoints[0]);
            line.DrawLine(g, pen);
        }
        public override bool IsPointInShape(PointF point)
        {
            bool inside = false;
            int n = shapePoints.Length;

            //Check using Ray-Casting method
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                if (((shapePoints[i].Y > point.Y) != (shapePoints[j].Y > point.Y)) &&
                    (point.X < (shapePoints[j].X - shapePoints[i].X) *
                    (point.Y - shapePoints[i].Y) / (shapePoints[j].Y - shapePoints[i].Y) + shapePoints[i].X))
                {
                    inside = !inside;
                }
            }

            return inside;
        }
        public override Shape Clone()
        {
            //Return new Triangle object of shapePoints
            var clone = new Triangle();
            clone.SetPoints(shapePoints);
            return clone;
        }
    }
    class Circle : Shape
    {
        public Circle()
        {
            shapePoints = new PointF[2];
        }
        public override void ConstructShape(Point[] selectedPoints)
        {
            //Shape Points are equal to Selected Points
            shapePoints[0] = selectedPoints[0];
            shapePoints[1] = selectedPoints[1];
        }
        public override void DrawShape(Graphics g, Pen pen)
        {
            //Calculate center point
            int centerX = (int)((shapePoints[0].X + shapePoints[1].X) / 2);
            int centerY = (int)((shapePoints[0].Y + shapePoints[1].Y) / 2);

            //Calculate radius with integer math
            int dx = (int)(shapePoints[1].X - shapePoints[0].X);
            int dy = (int)(shapePoints[1].Y - shapePoints[0].Y);
            int radius = (int)Math.Sqrt(dx * dx + dy * dy) / 2;

            //Initialise variables
            int x = 0;
            int y = radius;
            int d = 3 - 2 * radius;

            while (x <= y)
            {
                //Draw pixels in all 8 octants
                PutPixel(g, pen, new Point(centerX + x, centerY + y));
                PutPixel(g, pen, new Point(centerX + y, centerY + x));
                PutPixel(g, pen, new Point(centerX + y, centerY - x));
                PutPixel(g, pen, new Point(centerX + x, centerY - y));
                PutPixel(g, pen, new Point(centerX - x, centerY - y));
                PutPixel(g, pen, new Point(centerX - y, centerY - x));
                PutPixel(g, pen, new Point(centerX - y, centerY + x));
                PutPixel(g, pen, new Point(centerX - x, centerY + y));

                //Decision logic
                if (d <= 0)
                {
                    d = d + 4 * x + 6;
                }
                else
                {
                    d = d + 4 * (x - y) + 10;
                    y--;
                }
                x++;
            }
        }
        private void PutPixel(Graphics g, Pen pen, Point pixel)
        {
            //Fill a single pixel at coordinates
            Brush aBrush = new SolidBrush(pen.Color);
            g.FillRectangle(aBrush, pixel.X, pixel.Y, 1, 1);
        }
        public override bool IsPointInShape(PointF point)
        {
            //Get centre point
            PointF centrePoint = GetCentrePoint();

            //Calculate radius length
            double radius = Math.Sqrt(Math.Pow(Math.Abs(shapePoints[1].X - shapePoints[0].X), 2) + Math.Pow(Math.Abs(shapePoints[1].Y - shapePoints[0].Y), 2)) / 2;

            //Calculate change in x and y from centre point
            float dx = point.X - centrePoint.X;
            float dy = point.Y - centrePoint.Y;

            //Euclidean distance of point without sqaure root
            float distanceSquared = dx * dx + dy * dy;

            //Must be less or equal to Euclidean distance of diameter without square root
            return distanceSquared <= radius * radius;
        }
        public override Shape Clone()
        {
            //Return new Circle object of shapePoints
            var clone = new Circle();
            clone.SetPoints(shapePoints);
            return clone;
        }
    }
    class Line
    {
        PointF point1;
        PointF point2;

        public Line()
        {
            point1 = new Point();
            point2 = new Point();
        }
        public Line(PointF setPoint1, PointF setPoint2)
        {
            point1 = setPoint1;
            point2 = setPoint2;
        }
        public void SetPoints(PointF setPoint1, PointF setPoint2)
        {
            point1 = setPoint1;
            point2 = setPoint2;
        }
        public Point[] GetPoints()
        {
            return new Point[]
            {
                new Point((int)point1.X, (int)point1.Y),
                new Point((int)point2.X, (int)point2.Y)
            };
        }
        public void DrawLine(Graphics g, Pen pen)
        {
            //Use integer points for pixel-level drawing
            int x0 = (int)point1.X;
            int y0 = (int)point1.Y;
            int x1 = (int)point2.X;
            int y1 = (int)point2.Y;
            
            //Change in x,y from start to end points
            int dx = Math.Abs(x0 - x1);
            int dy = Math.Abs(y0 - y1);

            //Step of x,y positive or negative
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;

            //Calculate axis difference
            int offset = dx - dy;

            while (!(x0 == x1 && y0 == y1))
            {
                PutPixel(g, pen, new Point(x0, y0));

                //Scale difference
                int e2 = 2 * offset;

                //Check Y axis offset
                if (e2 > -dy)
                {
                    offset -= dy;
                    x0 += sx;
                }
                //Check X axis offset
                if (e2 < dx)
                {
                    offset += dx;
                    y0 += sy;
                }
            }
        }
        private void PutPixel(Graphics g, Pen pen, PointF pixel)
        {
            //Fill a single pixel at coordinates
            Brush aBrush = new SolidBrush(pen.Color);
            g.FillRectangle(aBrush, pixel.X, pixel.Y, 1, 1);
        }
    }
}


