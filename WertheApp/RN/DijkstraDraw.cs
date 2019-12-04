using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Diagnostics; //Debug.WriteLine("");
using System.Collections.Generic;

namespace WertheApp.RN
{
    public class DijkstraDraw
    {
        //VARIABLES
        //weight values
        private static String weightUV, weightUX, weightUW,
        weightUY, weightZW, weightZY,
        weightZV, weightZX, weightVX,
        weightVY, weightVW, weightXW,
        weightXY, weightYW;

        //center points for weights
        private static SKPoint wUV, wUX, wUW,
        wUY, wZW, wZY,
        wZV, wZX, wVX,
        wVY, wVW, wXW,
        wXY, wYW;

        //center points for router
        private static SKPoint routerZ, routerU, routerV,
        routerX, routerW, routerY;

        private static int networkNumber;
        private static SKCanvas canvas;
        private static SKCanvasView skiaview;
        private static float xe, ye, ye2;
        private static int currentStep;

        private static SKPaint sk_Background, sk_PaintBlack, sk_RouterText,
        sk_RouterContour, sk_RouterFill, sk_WeightsText, sk_TableCaption,
            sk_PaintBlackThin, sk_Background2;
        private static float textSize, strokeWidth;


        //CONSTRUCTOR
        public DijkstraDraw(String[] a, int n)
        {
            SetWeights(a[0], a[1], a[2], a[3], a[4], a[5], a[6], a[7], a[8], a[9], a[10], a[11], a[12], a[13]);
            SetNetworkNumber(n);

            switch (networkNumber)
            {
                case 1: DijkstraAlgorithm.BuildNetwork1(a); break;
                case 2: DijkstraAlgorithm.BuildNetwork2(a); break;
                case 3: DijkstraAlgorithm.BuildNetwork3(a); break;
                case 4: DijkstraAlgorithm.BuildNetwork4(a); break;
            }
            
            DijkstraAlgorithm.Initialize();
            DijkstraAlgorithm.CreateTableValuesArray();

            // crate the canvas
            skiaview = new SKCanvasView();
            skiaview.PaintSurface += PaintSurface;

            textSize = 5;
            strokeWidth = 0.2f;


            
           
        }


        //METHODS
        // do the drawing
        void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            //canvas object
            SKSurface surface = e.Surface;
            canvas = surface.Canvas;

            //Important! Otherwise the drawing will look messed up in iOS
            if (canvas != null) { canvas.Clear(); }

            //calculate some stuff and make the paint
            CalculateNeededNumbers();
            MakeSKPaint(); //depends on xe and ye and therfore has to be called after they were initialized

            /*********************HERE GOES THE DRAWING************************/
            DrawBackground(canvas);
            DrawTable();

            //draw Network
            DrawConnections();
            DrawRouters();
            DrawWeights();

            String[,] tableContents = DijkstraAlgorithm.GetTableValues();
            for (int i = 0; i <= currentStep; i++) {
                int round = Int32.Parse(tableContents[i, 1]);

                float yValueContent = 0;
                switch (round)
                {
                    case 0: yValueContent = 119.2f; break;
                    case 1: yValueContent = 129.2f; break;
                    case 2: yValueContent = 139.2f; break;
                    case 3: yValueContent = 149.2f; break;
                    case 4: yValueContent = 159.2f; break;
                    case 5: yValueContent = 169.2f; break;
                }
                canvas.DrawText(tableContents[i, 2], new SKPoint(15 * xe, yValueContent * ye2), sk_TableCaption);
                canvas.DrawText(tableContents[i, 3], new SKPoint(35 * xe, yValueContent * ye2), sk_TableCaption);
                canvas.DrawText(tableContents[i, 4], new SKPoint(47.5f * xe, yValueContent * ye2), sk_TableCaption);
                canvas.DrawText(tableContents[i, 5], new SKPoint(60 * xe, yValueContent * ye2), sk_TableCaption);
                canvas.DrawText(tableContents[i, 6], new SKPoint(72.5f * xe, yValueContent * ye2), sk_TableCaption);
                canvas.DrawText(tableContents[i, 7], new SKPoint(85 * xe, yValueContent * ye2), sk_TableCaption);

            }

            //execute all drawing actions
            canvas.Flush();
        }

        /**********************************************************************
        *********************************************************************/
        public static void SetNetworkNumber(int n)
        {
            networkNumber = n;
        }

        /**********************************************************************
        *********************************************************************/
        public static void DrawBackground(SKCanvas canvas)
        {
            SKRect sk_rBackground = new SKRect(00 * xe, 0 * ye2, 100 * xe, 100 * ye2); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground, sk_Background); //left, top, right, bottom, color
            SKRect sk_rBackground2 = new SKRect(00 * xe, 174 * ye2, 100 * xe, 200 * ye2); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground2, sk_Background2); //left, top, right, bottom, color
            canvas.DrawLine(new SKPoint(0 * xe, 100 * ye2), new SKPoint(100 * xe, 100 * ye2), sk_PaintBlack);

        }
        /**********************************************************************
        *********************************************************************/
        public static void DrawTable()
        {
            float cap = 109.2f;
            canvas.DrawText("Step", new SKPoint(5 * xe, cap * ye2), sk_TableCaption);
            canvas.DrawText("N'", new SKPoint(15 * xe, cap * ye2), sk_TableCaption);
            canvas.DrawText("D(v)", new SKPoint(35 * xe, cap * ye2), sk_TableCaption);
            canvas.DrawText("D(w)", new SKPoint(47.5f * xe, cap * ye2), sk_TableCaption);
            canvas.DrawText("D(x)", new SKPoint(60 * xe, cap * ye2), sk_TableCaption);
            canvas.DrawText("D(x)", new SKPoint(72.5f * xe, cap * ye2), sk_TableCaption);
            canvas.DrawText("D(x)", new SKPoint(85 * xe, cap * ye2), sk_TableCaption);

            canvas.DrawLine(new SKPoint(1 * xe, 112 * ye2), new SKPoint(99 * xe, 112 * ye2), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(1 * xe, 122 * ye2), new SKPoint(99 * xe, 122 * ye2), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(1 * xe, 132 * ye2), new SKPoint(99 * xe, 132 * ye2), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(1 * xe, 142 * ye2), new SKPoint(99 * xe, 142 * ye2), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(1 * xe, 152 * ye2), new SKPoint(99 * xe, 152 * ye2), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(1 * xe, 162 * ye2), new SKPoint(99 * xe, 162 * ye2), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(0 * xe, 174 * ye2), new SKPoint(100 * xe, 174 * ye2), sk_PaintBlack);


            canvas.DrawText("Destination", new SKPoint(9.5f * xe, 182.2f * ye2), sk_TableCaption);
            canvas.DrawText("v", new SKPoint(35 * xe, 182.2f * ye2), sk_TableCaption);
            canvas.DrawText("w", new SKPoint(47.5f * xe, 182.2f * ye2), sk_TableCaption);
            canvas.DrawText("x", new SKPoint(60 * xe, 182.2f * ye2), sk_TableCaption);
            canvas.DrawText("y", new SKPoint(72.5f * xe, 182.2f * ye2), sk_TableCaption);
            canvas.DrawText("z", new SKPoint(85 * xe, 182.2f * ye2), sk_TableCaption);
            canvas.DrawText("Link", new SKPoint(5 * xe, 191.2f * ye2), sk_TableCaption);
            canvas.DrawLine(new SKPoint(1 * xe, 185 * ye2), new SKPoint(99 * xe, 185 * ye2), sk_PaintBlackThin);

            canvas.DrawText("0", new SKPoint(5 * xe, 119.2f * ye2), sk_TableCaption);
            canvas.DrawText("1", new SKPoint(5 * xe, 129.2f * ye2), sk_TableCaption);
            canvas.DrawText("2", new SKPoint(5 * xe, 139.2f * ye2), sk_TableCaption);
            canvas.DrawText("3", new SKPoint(5 * xe, 149.2f * ye2), sk_TableCaption);
            canvas.DrawText("4", new SKPoint(5 * xe, 159.2f * ye2), sk_TableCaption);
            canvas.DrawText("5", new SKPoint(5 * xe, 169.2f * ye2), sk_TableCaption);

        }

      
        /**********************************************************************
        *********************************************************************/
        static void CalculateNeededNumbers()
        {
            /*important: the coordinate system starts in the upper left corner*/
            float lborder = canvas.LocalClipBounds.Left;
            float tborder = canvas.LocalClipBounds.Top;
            float rborder = canvas.LocalClipBounds.Right;
            float bborder = canvas.LocalClipBounds.Bottom;

            xe = rborder / 100; //using the variable surfacewidth instead would mess everything up
            ye = bborder / 100;
            ye2 = bborder / 200;

            // define center point for router
            routerZ = new SKPoint(90 * xe, 50 * ye2);
            routerU = new SKPoint(10 * xe, 50 * ye2);
            routerV = new SKPoint(35 * xe, 25 * ye2);
            routerW = new SKPoint(65 * xe, 25 * ye2);
            routerX = new SKPoint(35 * xe, 75 * ye2);
            routerY = new SKPoint(65 * xe, 75 * ye2);

            //define points for weights
            wUV = new SKPoint(20 * xe, 36.5f * ye2);
            wUX = new SKPoint(20 * xe, 68.5f * ye2);
            wZW = new SKPoint(80 * xe, 36.5f * ye2);
            wZY = new SKPoint(80 * xe, 68.5f * ye2);
            wVW = new SKPoint(50 * xe, 22.5f * ye2);
            wXY = new SKPoint(50 * xe, 82.5f * ye2);
            wVY = new SKPoint(41.5f * xe, 45.5f * ye2);
            wXW = new SKPoint(58.5f * xe, 45.5f * ye2);
            wVX = new SKPoint(31.5f * xe, 50 * ye2);
            wYW = new SKPoint(68.5f * xe, 50 * ye2);
            wUW = new SKPoint(15 * xe, 23.5f * ye2);
            wUY = new SKPoint(15 * xe, 80 * ye2);
            wZX = new SKPoint(85 * xe, 80 * ye2);
            wZV = new SKPoint(85 * xe, 23.5f * ye2);
        }

        /**********************************************************************
        *********************************************************************/
        public void DrawRouters()
        {
            DrawRouter(routerZ, "Z");
            DrawRouter(routerU, "U");
            DrawRouter(routerV, "V");
            DrawRouter(routerW, "W");
            DrawRouter(routerX, "X");
            DrawRouter(routerY, "Y");
        }

        /**********************************************************************
        *********************************************************************/
        void DrawRouter(SKPoint router, String name)
        {
            float radius = 40;

            //router
            canvas.DrawCircle(router, radius, sk_RouterFill);
            canvas.DrawCircle(router, radius, sk_RouterContour);

            //letter on router
            canvas.DrawText(name, router.X, router.Y + 20, sk_RouterText);

        }

        /**********************************************************************
        *********************************************************************/
        public void DrawConnections()
        {
            switch (networkNumber)
            {
                case 1:
                    DrawConnectionsNetwork1();
                    break;
                case 2:
                    DrawConnectionsNetwork2();
                    break;
                case 3:
                    DrawConnectionsNetwork3();
                    break;
                case 4:
                    DrawConnectionsNetwork4();
                    break;
            }
        }

        /**********************************************************************
        *********************************************************************/
        void DrawConnectionsNetwork1()
        {
            DrawConnection(routerX, routerU);
            DrawConnection(routerX, routerW);
            DrawConnection(routerX, routerY);
            DrawConnection(routerU, routerV);
            //DrawConnections(canvas, routerV, routerX);
            DrawConnection(routerV, routerW);
            DrawConnection(routerV, routerY);
            //DrawConnections(canvas, routerW, routerY);
            DrawConnection(routerW, routerZ);
            DrawConnection(routerZ, routerY);
        }

        /**********************************************************************
        *********************************************************************/
        void DrawConnectionsNetwork2()
        {
            DrawConnection(routerX, routerU);
            DrawConnection(routerX, routerW);
            DrawConnection(routerX, routerY);
            DrawConnection(routerU, routerV);
            DrawConnection(routerV, routerX);
            DrawConnection(routerV, routerW);
            //DrawConnections(canvas, routerV, routerY);
            DrawConnection(routerW, routerY);
            DrawConnection(routerW, routerZ);
            DrawConnection(routerZ, routerY);
            SKPoint p = new SKPoint(15 * xe, -15 * ye2);
            SKPath curveUW = new SKPath();
            curveUW.MoveTo(routerU);
            curveUW.CubicTo(routerU, p, routerW);
            canvas.DrawPath(curveUW, sk_RouterContour);
        }

        /**********************************************************************
        *********************************************************************/
        void DrawConnectionsNetwork3()
        {
            DrawConnection(routerX, routerU);
            DrawConnection(routerX, routerW);
            DrawConnection(routerX, routerY);
            DrawConnection(routerU, routerV);
            DrawConnection(routerV, routerX);
            DrawConnection(routerV, routerW);
            //DrawConnections(canvas, routerV, routerY);
            DrawConnection(routerW, routerY);
            DrawConnection(routerW, routerZ);
            DrawConnection(routerZ, routerY);
            SKPoint p = new SKPoint(15 * xe, 115 * ye2);
            SKPath curveUY = new SKPath();
            curveUY.MoveTo(routerU);
            curveUY.CubicTo(routerU, p, routerY);
            canvas.DrawPath(curveUY, sk_RouterContour);
        }

        /**********************************************************************
        *********************************************************************/
        void DrawConnectionsNetwork4()
        {
            DrawConnection(routerX, routerU);
            //DrawConnections(canvas, routerX, routerW);
            DrawConnection(routerX, routerY);
            DrawConnection(routerU, routerV);
            DrawConnection(routerV, routerX);
            DrawConnection(routerV, routerW);
            //DrawConnections(canvas, routerV, routerY);
            DrawConnection(routerW, routerY);
            DrawConnection(routerW, routerZ);
            DrawConnection(routerZ, routerY);

            SKPoint p = new SKPoint(15 * xe, -15 * ye2);
            SKPath curveUW = new SKPath();
            curveUW.MoveTo(routerU);
            curveUW.CubicTo(routerU, p, routerW);
            canvas.DrawPath(curveUW, sk_RouterContour);

            SKPoint p2 = new SKPoint(15 * xe, 115 * ye2);
            SKPath curveUY = new SKPath();
            curveUY.MoveTo(routerU);
            curveUY.CubicTo(routerU, p2, routerY);
            canvas.DrawPath(curveUY, sk_RouterContour);

            SKPoint p3 = new SKPoint(85 * xe, -15 * ye2);
            SKPath curveZV = new SKPath();
            curveZV.MoveTo(routerZ);
            curveZV.CubicTo(routerZ, p3, routerV);
            canvas.DrawPath(curveZV, sk_RouterContour);

            SKPoint p4 = new SKPoint(85 * xe, 115 * ye2);
            SKPath curveZX = new SKPath();
            curveZX.MoveTo(routerZ);
            curveZX.CubicTo(routerZ, p4, routerX);
            canvas.DrawPath(curveZX, sk_RouterContour);
        }

        /**********************************************************************
        *********************************************************************/
        void DrawConnection(SKPoint a, SKPoint b)
        {
            canvas.DrawLine(a, b, sk_RouterContour);
        }

        /**********************************************************************
        *********************************************************************/
        public void SetWeights(String uv, String ux, String uw,
        String uy, String zw, String zy,
        String zv, String zx, String vx,
        String vy, String vw, String xw,
        String xy, String yw)
        {
            weightUV = uv;
            weightUX = ux;
            weightUW = uw;
            weightUY = uy;
            weightZW = zw;
            weightZY = zy;
            weightZV = zv;
            weightZX = zx;
            weightVX = vx;
            weightVY = vy;
            weightVW = vw;
            weightXW = xw;
            weightXY = xy;
            weightYW = yw;
        }

        /**********************************************************************
        *********************************************************************/
        public void DrawWeights()
        {
            switch (networkNumber)
            {
                case 1:
                    DrawWeightsNetwork1();
                    break;
                case 2:
                    DrawWeightsNetwork2();
                    break;
                case 3:
                    DrawWeightsNetwork3();
                    break;
                case 4:
                    DrawWeightsNetwork4();
                    break;
            }
        }

        /**********************************************************************
       *********************************************************************/
        void DrawWeightsNetwork1()
        {
            canvas.DrawText(weightUV, wUV, sk_WeightsText);
            canvas.DrawText(weightUX, wUX, sk_WeightsText);
            canvas.DrawText(weightZW, wZW, sk_WeightsText);
            canvas.DrawText(weightZY, wZY, sk_WeightsText);
            canvas.DrawText(weightXY, wXY, sk_WeightsText);
            canvas.DrawText(weightVW, wVW, sk_WeightsText);
            canvas.DrawText(weightVY, wVY, sk_WeightsText);
            canvas.DrawText(weightXW, wXW, sk_WeightsText);

        }

        /**********************************************************************
        *********************************************************************/
        void DrawWeightsNetwork2()
        {
            canvas.DrawText(weightUV, wUV, sk_WeightsText);
            canvas.DrawText(weightUX, wUX, sk_WeightsText);
            canvas.DrawText(weightZW, wZW, sk_WeightsText);
            canvas.DrawText(weightZY, wZY, sk_WeightsText);
            canvas.DrawText(weightXY, wXY, sk_WeightsText);
            canvas.DrawText(weightVW, wVW, sk_WeightsText);
            canvas.DrawText(weightXW, wXW, sk_WeightsText);
            canvas.DrawText(weightVX, wVX, sk_WeightsText);
            canvas.DrawText(weightYW, wYW, sk_WeightsText);
            canvas.DrawText(weightUW, wUW, sk_WeightsText);
        }

        /**********************************************************************
        *********************************************************************/
        void DrawWeightsNetwork3()
        {
            canvas.DrawText(weightUV, wUV, sk_WeightsText);
            canvas.DrawText(weightUX, wUX, sk_WeightsText);
            canvas.DrawText(weightZW, wZW, sk_WeightsText);
            canvas.DrawText(weightZY, wZY, sk_WeightsText);
            canvas.DrawText(weightXY, wXY, sk_WeightsText);
            canvas.DrawText(weightVW, wVW, sk_WeightsText);
            canvas.DrawText(weightXW, wXW, sk_WeightsText);
            canvas.DrawText(weightVX, wVX, sk_WeightsText);
            canvas.DrawText(weightYW, wYW, sk_WeightsText);
            canvas.DrawText(weightUY, wUY, sk_WeightsText);
        }

        /**********************************************************************
        *********************************************************************/
        void DrawWeightsNetwork4()
        {
            canvas.DrawText(weightUV, wUV, sk_WeightsText);
            canvas.DrawText(weightUX, wUX, sk_WeightsText);
            canvas.DrawText(weightZW, wZW, sk_WeightsText);
            canvas.DrawText(weightZY, wZY, sk_WeightsText);
            canvas.DrawText(weightXY, wXY, sk_WeightsText);
            canvas.DrawText(weightVW, wVW, sk_WeightsText);
            canvas.DrawText(weightVX, wVX, sk_WeightsText);
            canvas.DrawText(weightYW, wYW, sk_WeightsText);
            canvas.DrawText(weightUW, wUW, sk_WeightsText);
            canvas.DrawText(weightUY, wUY, sk_WeightsText);
            canvas.DrawText(weightZV, wZV, sk_WeightsText);
            canvas.DrawText(weightZX, wZX, sk_WeightsText);

        }

        /**********************************************************************
        *********************************************************************/
        public static SKCanvasView ReturnCanvas()
        {
            return skiaview;
        }

        /**********************************************************************
        *********************************************************************/
        public static void SetCurrentStep(int s)
        {
            currentStep = s;
        }
        /**********************************************************************
        *********************************************************************/
        //redraws the canvas
        public static void Paint()
        {
            // update the canvas when the data changes
            skiaview.InvalidateSurface();

        }

        /**********************************************************************
        *********************************************************************/
        static private void MakeSKPaint(){

            sk_Background = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(67, 110, 238).WithAlpha(30)
            };

            sk_Background2 = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(238, 130, 238).WithAlpha(30)
            };

            sk_PaintBlackThin = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth / 1f * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(0, 0, 0) //black
            };

            sk_PaintBlack = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * 2f * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(0, 0, 0) //black
            };

            sk_RouterText = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.StrokeAndFill,
                TextSize = 45,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center,
                IsVerticalText = false
            };

            sk_TableCaption = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.StrokeAndFill,
                TextSize = 32,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center,
                IsVerticalText = false
            };

            sk_RouterFill = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(170, 170, 170)
            };

            sk_RouterContour = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(100, 100, 100)
            };

            sk_WeightsText = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.StrokeAndFill,
                TextSize = 40,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center,
                IsVerticalText = false
            };
        }
    }
}
