using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;

namespace project
{

    public class ball
    {
        public RectangleF rcDst, rcSrc;
        public Bitmap ball_img;
        public float x_clickS,y_clickS;  
        public float x_clickE,y_clickE;
        public bool move_ball = false;
        public float time_inc = 0.0f;
        public PointF ballPoint;
        public int rand;
    }
    public class BezierCurve
    {

        public List<Point> ControlPoints;

        public float t_inc = 0.001f;

        public Color cl = Color.Red;
        public Color clr1 = Color.Black;
        public Color ftColor = Color.Black;




        public BezierCurve()
        {
            ControlPoints = new List<Point>();
        }

        /*Done to calculate factorial*/
        private double Factorial(int n)
        {
            double res = 1.0f;

            for (double i = 2; i <= n; i++)
                res *= i;

            return res;
        }
        /*Done to calculate nCi*/
        private double C(int n, int i)
        {
            double res = Factorial(n) / (Factorial(i) * Factorial(n - i));
            return res;
        }
        /*Done to calculate k(it)*/
        private double Calc_B(float t, int i)
        {
            int n = ControlPoints.Count - 1;
            double res = C(n, i) *
                            Math.Pow((1 - t), (n - i)) *
                            Math.Pow(t, i);
            return res;
        }

        public Point GetPoint(int i)
        {
            return ControlPoints[i];
        }
        /*Done to get P(t)*/
        public PointF CalcCurvePointAtTime(float t)
        {
            PointF pt = new PointF();
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                float B = (float)Calc_B(t, i);
                pt.X += B * ControlPoints[i].X;
                pt.Y += B * ControlPoints[i].Y;
            }

            return pt;
        }
        /*Done draw control points*/
        private void DrawControlPoints(Graphics g)
        {
            Font Ft = new Font("System", 10);
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                g.FillEllipse(new SolidBrush(clr1),
                                ControlPoints[i].X,
                                ControlPoints[i].Y, 10, 10);

                g.DrawString("P# " + i + " x " + (ControlPoints[i].X) + " y " + (ControlPoints[i].Y), Ft, new SolidBrush(Color.White), ControlPoints[i].X - 15, ControlPoints[i].Y - 15);
            }
        }
        /*Done to get control points*/
        public int isCtrlPoint(int XMouse, int YMouse)
        {
            Rectangle rc;
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                rc = new Rectangle(ControlPoints[i].X - 5, ControlPoints[i].Y - 5, 10, 10);
                if (XMouse >= rc.Left && XMouse <= rc.Right && YMouse >= rc.Top && YMouse <= rc.Bottom)
                {
                    return i;
                }
            }
            return -1;
        }
        /*Done we modify the position for the selected point*/
        public void ModifyCtrlPoint(int i, int XMouse, int YMouse)
        {
            Point p = ControlPoints[i];

            p.X = XMouse;
            p.Y = YMouse;
            ControlPoints[i] = p;
        }
        /*Done set the cntrl pts*/
        public void SetControlPoint(Point pt)
        {
            ControlPoints.Add(pt);
        }


        /*Done we draw the curve pts*/
        private void DrawCurvePoints(Graphics g)
        {
            if (ControlPoints.Count <= 0)
                return;

            PointF curvePoint;
            for (float t = 0.0f; t <= 1.0; t += t_inc)
            {
                /* curvePoint =P(t)*/
                curvePoint = CalcCurvePointAtTime(t);
                g.FillEllipse(new SolidBrush(cl),
                                curvePoint.X - 4, curvePoint.Y - 4,
                                8, 8);
            }
        }
        /*Done we draw the curve */
        public void DrawCurve(Graphics g)
        {
            DrawControlPoints(g);
            DrawCurvePoints(g);
        }


    }
    public class Frog
    {
        public float x, y;
        public Bitmap frog_img = new Bitmap("zuma.png");
        public RectangleF rcDst, rcSrc;

    }

    public class background
    {
        public Rectangle rcsrc, rdst;
        public Bitmap bg=new Bitmap("level1_bg.jpg");
    }
    public partial class Form1 : Form
    {
        Bitmap off;
        Timer Timer = new Timer();
        List<Frog> frog = new List<Frog>();
        public List<background> bgs = new List<background>();
        public List<ball> balls = new List<ball>();
        public List<ball> ball_curve = new List<ball>();
        public List<BezierCurve> curve1 = new List<BezierCurve>();
        public BezierCurve curve = new BezierCurve();
        int numOfCtrlPoints = 0;
        float rotationAngle = 0f;
        float mouseX;
        float mouseY;
        float xmouse;
        float ymouse;
        enum Modes { CTRL_POINTS, DRAG };
        Modes CurrentMouseMode = Modes.CTRL_POINTS;
        int indexCurrDragNode = -1;
        Random random = new Random();
        bool flag_start_ball_curve = false;
        int r;
        int ctTick = 0;
        int counter = 0;
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.Load += Form1_Load;
            this.Paint += Form1_Paint;
            this.MouseMove += Form1_MouseMove;
            this.MouseDown += Form1_MouseDown;
            this.MouseUp += Form1_MouseUp;
            Timer.Tick += Timer_Tick;
            Timer.Start();
            this.KeyDown += Form1_KeyDown;
           

        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (CurrentMouseMode == Modes.DRAG)
            {
                indexCurrDragNode = -1;
                DrawDubb();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
/*            if(e.KeyCode == Keys.Space)
            {
                if (CurrentMouseMode == Modes.DRAG)
                    CurrentMouseMode = Modes.CTRL_POINTS;
                else
                    CurrentMouseMode = Modes.DRAG;
            }*/
/*            if(e.KeyCode==Keys.Enter)
            {
                create_ball_curve();
            }*/
            DrawDubb();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {

           balls[balls.Count - 1].move_ball = true;
           balls[balls.Count - 1].x_clickE = e.X;
           balls[balls.Count - 1].y_clickE = e.Y;



            balls[balls.Count - 1].x_clickS = balls[balls.Count-1].rcDst.X;
            balls[balls.Count - 1].y_clickS = balls[balls.Count - 1].rcDst.Y;
/*            curve.SetControlPoint(new Point(e.X, e.Y));
            numOfCtrlPoints++;
            this.Text += " " + e.X + " x " + e.Y + " y";*/
          
/*            for(int i=0;i<curve1.Count;i++)
            {
                if (CurrentMouseMode == Modes.CTRL_POINTS)
                {
                    curve1[i].SetControlPoint(new Point(e.X, e.Y));
                    numOfCtrlPoints++;
                }
                else if (CurrentMouseMode == Modes.DRAG)
                {
                    indexCurrDragNode = curve1[i].isCtrlPoint(e.X, e.Y);
                }
            }*/
            
            create_ball();
            DrawDubb();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {

             mouseX = e.X - this.ClientSize.Width/2;
             mouseY = e.Y - this.ClientSize.Height / 2;
             xmouse = e.X;
             ymouse = e.Y;
            rotationAngle = (float)(Math.Atan2(mouseY, mouseX) * 180 / Math.PI);
            /*DrawDubb();*/
        }

        private void Timer_Tick(object sender, EventArgs e)
        {   
            if(flag_start_ball_curve==true && ctTick%5==0)
            {
                create_ball_curve();
            }
            DDA();
            ctTick++;
            DrawDubb();
        }
        void DDA()
        {
            for(int i=0;i<balls.Count;i++)
            {
           
                if (balls[i].move_ball ==true) 
                {
                 
                    float dx = balls[i].x_clickE - balls[i].x_clickS;
                    float dy = balls[i].y_clickE - balls[i].y_clickS;
                    float slope = dy / dx;
                    if (Math.Abs(dx) > Math.Abs(dy))
                    {
                        if (balls[i].x_clickS < balls[i].x_clickE)
                        {
                            check_hitBall(i);
                            balls[i].rcDst.X += 10;
                            balls[i].rcDst.Y += slope * 10;
                        }
                        else
                        {
                            check_hitBall(i);
                            balls[i].rcDst.X -= 10;
                            balls[i].rcDst.Y -= slope * 10;
                        }
                    }
                    else
                    {
                        if (balls[i].y_clickS < balls[i].y_clickE)
                        {
                            check_hitBall(i);
                            balls[i].rcDst.Y += 10;
                            balls[i].rcDst.X += (1 / slope * 10);
                        }
                        else
                        {
                            check_hitBall(i);
                            balls[i].rcDst.Y -= 10;
                            balls[i].rcDst.X -= (1 / slope * 10);
                        }
                    }
                }
            }
            /*DrawDubb();*/
        }
        void check_hitBall(int index)
        {

            for(int i=0;i<ball_curve.Count;i++) 
            {
                if (balls[index].rand == ball_curve[i].rand) 
                {
                    PointF ball_curveS = new PointF(ball_curve[i].rcDst.X, ball_curve[i].rcDst.Y);
                    PointF ball_curveE = new PointF(ball_curve[i].rcDst.X + ball_curve[i].rcDst.Width,
                        ball_curve[i].rcDst.Y + ball_curve[i].rcDst.Height);
                    PointF ballS = new PointF(balls[index].rcDst.X, balls[index].rcDst.Y);
                    PointF ballE = new PointF(balls[index].rcDst.X + balls[index].rcDst.Width, balls[index].rcDst.Y + balls[index].rcDst.Height);
                    if (((ball_curveS.X > ballS.X && ball_curveS.X < ballE.X) ||
                        (ball_curveE.X > ballS.X && ball_curveE.X < ballE.X)) &&
                        ((ball_curveS.Y > ballS.Y && ball_curveS.Y < ballE.Y) ||
                        (ball_curveE.Y > ballS.Y && ball_curveE.Y < ballE.Y)))
                    {
                        
                       
                        
                        while(true)
                        {
                            if (ball_curve[i].rand == balls[index].rand)
                            {
                                ball_curve.RemoveAt(i);
                            }
                            else
                            {
                                break;
                            }
                        }
                        int val = i;
                        while (true)
                        {
                            if (ball_curve[val].rand == balls[index].rand)
                            {
                                ball_curve.RemoveAt(val);
                            }
                            else
                            {
                                break;
                            }
                            val--;
                        }

                        balls.RemoveAt(index);

                    }
                }
               
            }
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(ClientSize.Width, ClientSize.Height);
            create_background();
            /*curve.SetControlPoint(new Point(942, 513));*/
            create_frog();
            create_curve1();
            create_ball();
            flag_start_ball_curve = true;
            DrawDubb();
        }
        void DrawDubb()
        {
            Graphics g = CreateGraphics();
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
        void DrawScene(Graphics g)
        {
            g.Clear(Color.Black);
            for (int i = 0; i < bgs.Count; i++)
            {
                g.DrawImage(bgs[i].bg, bgs[i].rdst, bgs[i].rcsrc, GraphicsUnit.Pixel);
            }
            AnimateFrog(g);
/*            for (int i = 0; i < curve1.Count; i++)
            {
                curve1[i].DrawCurve(g);
            }*/
            /*curve.DrawCurve(g);*/
            for (int i=0;i<ball_curve.Count;i++)
            {
                ball_curve[i].ballPoint = curve1[0].CalcCurvePointAtTime(ball_curve[i].time_inc);
                ball_curve[i].rcDst.X = ball_curve[i].ballPoint.X;  
                ball_curve[i].rcDst.Y = ball_curve[i].ballPoint.Y;
                if (ball_curve[i].time_inc<=1)
                {
                    g.DrawImage(ball_curve[i].ball_img, ball_curve[i].rcDst, ball_curve[i].rcSrc, GraphicsUnit.Pixel);
                }
/*                else
                {
                    Timer.Stop();
                }*/
                ball_curve[i].time_inc += 0.001f;
            }
            for (int i = 0; i < balls.Count; i++)
            {
                if (balls[i].move_ball == false)
                {
                    // Update the position of the ball based on the frog's position and rotation
                    float ballDistanceFromFrog = 30f; // Adjust this value to set the distance between the frog and the ball
                    float ballRotationOffset = 5f; // Adjust this value to set the initial rotation offset of the ball
                    float ballRotationAngle = rotationAngle + ballRotationOffset;
                    float ballX = frog[0].rcDst.X + frog[0].rcDst.Width / 2 + ballDistanceFromFrog * (float)Math.Cos(ballRotationAngle * Math.PI / 180);
                    float ballY = frog[0].rcDst.Y + frog[0].rcDst.Height / 2 + ballDistanceFromFrog * (float)Math.Sin(ballRotationAngle * Math.PI / 180);
                    balls[i].rcDst.X = (int)(ballX - balls[i].rcDst.Width / 2);
                    balls[i].rcDst.Y = (int)(ballY - balls[i].rcDst.Height / 2);

                    g.DrawImage(balls[i].ball_img, balls[i].rcDst, balls[i].rcSrc, GraphicsUnit.Pixel);
                }
                else
                {
                    g.DrawImage(balls[i].ball_img, balls[i].rcDst, balls[i].rcSrc, GraphicsUnit.Pixel);
                }
            }
        }


        void AnimateFrog(Graphics g)
        {
            Pen pen = new Pen(Color.Yellow, 5);
            g.DrawLine(pen, frog[0].rcDst.X + frog[0].rcDst.Width / 2, frog[0].rcDst.Y + frog[0].rcDst.Height / 2, xmouse, ymouse);

            Bitmap bmp = new Bitmap(frog[0].frog_img.Width, frog[0].frog_img.Height);
            Graphics g2 = Graphics.FromImage(bmp);
            g2.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
            g2.RotateTransform(rotationAngle + 95);
            g2.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
            g2.DrawImage(frog[0].frog_img, new Point(0, 0));
            g.DrawImage(bmp, frog[0].rcDst, frog[0].rcSrc, GraphicsUnit.Pixel);

          
        }


        void create_frog()
        {
            Frog pnn = new Frog();
            pnn.rcSrc = new RectangleF(0, 0, pnn.frog_img.Width, pnn.frog_img.Height);
            pnn.rcDst = new RectangleF(this.ClientSize.Width / 2-50, this.ClientSize.Height / 2-80, pnn.frog_img.Width/2, pnn.frog_img.Height/2);
            frog.Add(pnn);
        }
        void create_background()
        {
            background pnn = new background();
            pnn.rcsrc = new Rectangle(0, 0, pnn.bg.Width, pnn.bg.Height);
            pnn.rdst = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            bgs.Add(pnn);
        }

        void create_ball()
        {
            ball pnn = new ball();
            r = random.Next(0, 5);
            pnn.ball_img = new Bitmap(r + ".png");
            pnn.rand = r;
            pnn.rcSrc = new RectangleF(0, 0, pnn.ball_img.Width, pnn.ball_img.Height);

            // Calculate the position of the ball based on the frog's position and rotation
            float ballX = frog[0].rcDst.X + frog[0].rcDst.Width / 2 - pnn.ball_img.Width / 16;
            float ballY = frog[0].rcDst.Y + frog[0].rcDst.Height / 2 - pnn.ball_img.Height / 16;

            pnn.rcDst = new RectangleF(ballX, ballY, pnn.ball_img.Width / 6, pnn.ball_img.Height / 6);
            balls.Add(pnn);
        }
        void create_curve1()
        {
            BezierCurve pnn = new BezierCurve();
            pnn.SetControlPoint(new Point(1121, 8));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1120, 172));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1125, 271));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1130, 376));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1150, 436));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1120, 490));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1100, 530));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1030, 580));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(980, 645));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(873, 690));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(814, 720));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(734, 750));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(640, 750));
            numOfCtrlPoints++;
            /*            pnn.SetControlPoint(new Point(640, 682));
                        numOfCtrlPoints++;*/
            /*pnn.SetControlPoint(new Point(660, 707));
            numOfCtrlPoints++;*/
            pnn.SetControlPoint(new Point(490, 750));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(410, 750));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(330, 750));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(250, 590));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(230, 570));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(160, 500));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(120, 417));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(100, 351));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(120, 260));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(180, 210));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(220, 130));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(250, 100));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(320, 70));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(420, 40));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(487, 30));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(571, 20));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(663, 20));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(718, 20));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(800, 30));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(850, 60));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(910, 70));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(970, 100));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1000, 157));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1050, 222));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1030, 254));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1070, 289));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1077, 320));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1080, 361));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1079, 398));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1020, 450));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1050, 490));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1100, 510));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(950, 650));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(764, 750));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(732, 753));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(613, 882));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(497, 881));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(330, 884));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(10, 450));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(20, 294));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(70, 110));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(70, 62));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(200, 28));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(632, 23));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(850, 20));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1150, 50));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1100, 130));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(1100, 413));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(974, 477));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(850, 600));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(787, 571));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(652, 572));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(518, 556));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(467, 510));
            numOfCtrlPoints++;
            pnn.SetControlPoint(new Point(445, 348));
            numOfCtrlPoints++;
            pnn.cl = Color.Blue;
            curve1.Add(pnn);
        }
        void create_ball_curve()
        {
            ball pnn = new ball();
            r = random.Next(0, 5);
            pnn.ballPoint = curve1[0].CalcCurvePointAtTime(pnn.time_inc);
            pnn.rand = r;
            pnn.ball_img = new Bitmap(r + ".png");
            pnn.rcSrc = new RectangleF(0, 0, pnn.ball_img.Width, pnn.ball_img.Height);
            pnn.rcDst = new RectangleF(pnn.ballPoint.X, pnn.ballPoint.Y, pnn.ball_img.Width / 6, pnn.ball_img.Height / 6);
            ball_curve.Add(pnn);
        }
    }
}
