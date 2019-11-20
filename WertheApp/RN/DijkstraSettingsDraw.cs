using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Diagnostics; //Debug.WriteLine("");
using System.Collections.Generic;

namespace WertheApp.RN
{
    public class DijkstraSettingsDraw
    {
        //VARIABLES
        private SKCanvasView skiaview;
        private SKCanvas canvas;
        private readonly int id;

        private static List<DijkstraSettingsDraw> networkList;
        private static float xe, ye;

        private static SKPaint sk_blackText, sk_WeightsText,
            sk_RouterText, sk_RouterContour, sk_RouterFill, sk_test;
        private static float textSize;
        private static SKPoint routerZ, routerU, routerV, routerX, routerW, routerY;
        private static SKPoint wUV, wUX, wUW, wUY, wZW, wZY, wZV, wZX, wVX, wVY, wVW, wXW, wXY, wYW;

        //CONSTRUCTOR
        public DijkstraSettingsDraw(int id)
        {
            this.id = id;
            networkList = new List<DijkstraSettingsDraw>();
            networkList.Add(this);


            // crate the canvas
            this.skiaview = new SKCanvasView();
            this.skiaview.PaintSurface += PaintSurface;

            textSize = 5;
            //strokeWidth = 0.2f;
        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        // do the drawing
        void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            //canvas object
            SKSurface surface = e.Surface;
            canvas = surface.Canvas;

            //Important! Otherwise the drawing will look messed up in iOS
            if (canvas != null){ canvas.Clear();}

            //calculate some stuff and make the paint
            CalculateNeededNumbers(canvas);
            MakeSKPaint(); //depends on xe and ye and therfore has to be called after they were initialized

            /*********************HERE GOES THE DRAWING************************/
            SKRect sk_rBackground = new SKRect(00 * xe, 0 * ye, 100 * xe, 100 * ye); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground, sk_test); //left, top, right, bottom, color

            //draw connections
            switch (this.id)
            {
                case 1: DrawNetwork1();
                    break;
                case 2: DrawNetwork2();
                    break;
                case 3: DrawNetwork3();
                    break;
                case 4: DrawNetwork4();
                    break;
            }
       
            //draw routers
            DrawRouter(routerZ, "z");
            DrawRouter(routerU, "u");
            DrawRouter(routerV, "v");
            DrawRouter(routerW, "w");
            DrawRouter(routerX, "x");
            DrawRouter(routerY, "y");

            //draw values (weights)
            switch (this.id)
            {
                case 1:
                    DrawWeightsNetwork1("1", "2", "3", "4", "5", "6", "7", "8");
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
            //TODO: Think of Default Values
            //TODO: Values anpassen depending on wie die Gewichte eingegeben wurden
            //TODO: Add click event to weight locations

            //execute all drawing actions
            canvas.Flush();
        }

        /**********************************************************************
        *********************************************************************/
        public void SetDefaultWeights()
        {

        }

        /**********************************************************************
        *********************************************************************/
        void DrawWeightsNetwork1(String weightUV, String weightUX, String weightZW,
            String weightZY, String weightXY, String weightVW,
            String weightVY, String weightXW)
        {
            this.canvas.DrawText(weightUV, wUV, sk_WeightsText);
            this.canvas.DrawText(weightUX, wUX, sk_WeightsText);
            this.canvas.DrawText(weightZW, wZW, sk_WeightsText);
            this.canvas.DrawText(weightZY, wZY, sk_WeightsText);
            this.canvas.DrawText(weightXY, wXY, sk_WeightsText);
            this.canvas.DrawText(weightVW, wVW, sk_WeightsText);
            this.canvas.DrawText(weightVY, wVY, sk_WeightsText);
            this.canvas.DrawText(weightXW, wXW, sk_WeightsText);

        }

        /**********************************************************************
        *********************************************************************/
        void DrawWeightsNetwork2()
        {
            String weight = "0";
            this.canvas.DrawText(weight, wUV, sk_WeightsText);
            this.canvas.DrawText(weight, wUX, sk_WeightsText);
            this.canvas.DrawText(weight, wZW, sk_WeightsText);
            this.canvas.DrawText(weight, wZY, sk_WeightsText);
            this.canvas.DrawText(weight, wXY, sk_WeightsText);
            this.canvas.DrawText(weight, wVW, sk_WeightsText);
            this.canvas.DrawText(weight, wXW, sk_WeightsText);
            this.canvas.DrawText(weight, wVX, sk_WeightsText);
            this.canvas.DrawText(weight, wYW, sk_WeightsText);
            this.canvas.DrawText(weight, wUW, sk_WeightsText);
        }

        /**********************************************************************
        *********************************************************************/
        void DrawWeightsNetwork3()
        {
            String weight = "0";
            this.canvas.DrawText(weight, wUV, sk_WeightsText);
            this.canvas.DrawText(weight, wUX, sk_WeightsText);
            this.canvas.DrawText(weight, wZW, sk_WeightsText);
            this.canvas.DrawText(weight, wZY, sk_WeightsText);
            this.canvas.DrawText(weight, wXY, sk_WeightsText);
            this.canvas.DrawText(weight, wVW, sk_WeightsText);
            this.canvas.DrawText(weight, wXW, sk_WeightsText);
            this.canvas.DrawText(weight, wVX, sk_WeightsText);
            this.canvas.DrawText(weight, wYW, sk_WeightsText);
            this.canvas.DrawText(weight, wUY, sk_WeightsText);
        }

        /**********************************************************************
        *********************************************************************/
        void DrawWeightsNetwork4()
        {
            String weight = "0";
            this.canvas.DrawText(weight, wUV, sk_WeightsText);
            this.canvas.DrawText(weight, wUX, sk_WeightsText);
            this.canvas.DrawText(weight, wZW, sk_WeightsText);
            this.canvas.DrawText(weight, wZY, sk_WeightsText);
            this.canvas.DrawText(weight, wXY, sk_WeightsText);
            this.canvas.DrawText(weight, wVW, sk_WeightsText);
            this.canvas.DrawText(weight, wVX, sk_WeightsText);
            this.canvas.DrawText(weight, wYW, sk_WeightsText);
            this.canvas.DrawText(weight, wUW, sk_WeightsText);
            this.canvas.DrawText(weight, wUY, sk_WeightsText);
            this.canvas.DrawText(weight, wZV, sk_WeightsText);
            this.canvas.DrawText(weight, wZX, sk_WeightsText);

        }

        /**********************************************************************
        *********************************************************************/
        void DrawNetwork1()
        {
            DrawConnections(routerX, routerU);
            DrawConnections(routerX, routerW);
            DrawConnections(routerX, routerY);
            DrawConnections(routerU, routerV);
            //DrawConnections(canvas, routerV, routerX);
            DrawConnections(routerV, routerW);
            DrawConnections(routerV, routerY);
            //DrawConnections(canvas, routerW, routerY);
            DrawConnections(routerW, routerZ);
            DrawConnections(routerZ, routerY);
        }

        /**********************************************************************
        *********************************************************************/
        void DrawNetwork2()
        {
            DrawConnections(routerX, routerU);
            DrawConnections(routerX, routerW);
            DrawConnections(routerX, routerY);
            DrawConnections(routerU, routerV);
            DrawConnections(routerV, routerX);
            DrawConnections(routerV, routerW);
            //DrawConnections(canvas, routerV, routerY);
            DrawConnections(routerW, routerY);
            DrawConnections(routerW, routerZ);
            DrawConnections(routerZ, routerY);
            SKPoint p = new SKPoint(15 * xe, -15 * ye);
            SKPath curveUW = new SKPath();
            curveUW.MoveTo(routerU);
            curveUW.CubicTo(routerU, p, routerW);
            canvas.DrawPath(curveUW, sk_RouterContour);
        }

        /**********************************************************************
        *********************************************************************/
        void DrawNetwork3()
        {
            DrawConnections(routerX, routerU);
            DrawConnections(routerX, routerW);
            DrawConnections(routerX, routerY);
            DrawConnections(routerU, routerV);
            DrawConnections(routerV, routerX);
            DrawConnections(routerV, routerW);
            //DrawConnections(canvas, routerV, routerY);
            DrawConnections(routerW, routerY);
            DrawConnections(routerW, routerZ);
            DrawConnections(routerZ, routerY);
            SKPoint p = new SKPoint(15 * xe, 115 * ye);
            SKPath curveUY = new SKPath();
            curveUY.MoveTo(routerU);
            curveUY.CubicTo(routerU, p, routerY);
            canvas.DrawPath(curveUY, sk_RouterContour);
        }

        /**********************************************************************
        *********************************************************************/
        void DrawNetwork4()
        {
            DrawConnections(routerX, routerU);
            //DrawConnections(canvas, routerX, routerW);
            DrawConnections(routerX, routerY);
            DrawConnections(routerU, routerV);
            DrawConnections(routerV, routerX);
            DrawConnections(routerV, routerW);
            //DrawConnections(canvas, routerV, routerY);
            DrawConnections(routerW, routerY);
            DrawConnections(routerW, routerZ);
            DrawConnections(routerZ, routerY);

            SKPoint p = new SKPoint(15 * xe, -15 * ye);
            SKPath curveUW = new SKPath();
            curveUW.MoveTo(routerU);
            curveUW.CubicTo(routerU, p, routerW);
            canvas.DrawPath(curveUW, sk_RouterContour);

            SKPoint p2 = new SKPoint(15 * xe, 115 * ye);
            SKPath curveUY = new SKPath();
            curveUY.MoveTo(routerU);
            curveUY.CubicTo(routerU, p2, routerY);
            canvas.DrawPath(curveUY, sk_RouterContour);

            SKPoint p3 = new SKPoint(85 * xe, -15 * ye);
            SKPath curveZV = new SKPath();
            curveZV.MoveTo(routerZ);
            curveZV.CubicTo(routerZ, p3, routerV);
            canvas.DrawPath(curveZV, sk_RouterContour);

            SKPoint p4 = new SKPoint(85 * xe, 115 * ye);
            SKPath curveZX = new SKPath();
            curveZX.MoveTo(routerZ);
            curveZX.CubicTo(routerZ, p4, routerX);
            canvas.DrawPath(curveZX, sk_RouterContour);
        }

        /**********************************************************************
        *********************************************************************/
        void DrawRouter(SKPoint router, String name)
        {
            float radius = 60;
            float crossLength = radius / 3;
            float crossWidth = crossLength / 2;
      
            //router
            this.canvas.DrawCircle(router, radius, sk_RouterFill);
            this.canvas.DrawCircle(router, radius, sk_RouterContour);

            //x on router
            SKPath cross = new SKPath();
            cross.MoveTo(router.X, router.Y -10);
            cross.RLineTo(-crossLength, -crossLength);
            cross.RLineTo(-crossWidth, +crossWidth);
            cross.RLineTo(+crossLength, +crossLength);
            cross.RLineTo(-crossLength, +crossLength);
            cross.RLineTo(+crossWidth, +crossWidth);
            cross.RLineTo(+crossLength, -crossLength);
            cross.RLineTo(+crossLength, +crossLength);
            cross.RLineTo(+crossWidth, -crossWidth);
            cross.RLineTo(-crossLength, -crossLength);
            cross.RLineTo(+crossLength, -crossLength);
            cross.RLineTo(-crossWidth, -crossWidth);
            cross.RLineTo(-crossLength, +crossLength);
            canvas.DrawPath(cross, sk_RouterContour);

            //letter on router
            this.canvas.DrawText(name, router.X, router.Y + 50, sk_RouterText);

        }

        /**********************************************************************
        *********************************************************************/
        void DrawConnections(SKPoint a, SKPoint b)
        {
            this.canvas.DrawLine(a, b, sk_RouterContour);
            //make all connections and parameters will be connections for special networks
            //connection v - x

            //connection w - y

        }

        /**********************************************************************
        *********************************************************************/
        static void CalculateNeededNumbers(SKCanvas canvas)
        {
            /*important: the coordinate system starts in the upper left corner*/
            float lborder = canvas.LocalClipBounds.Left;
            float tborder = canvas.LocalClipBounds.Top;
            float rborder = canvas.LocalClipBounds.Right;
            float bborder = canvas.LocalClipBounds.Bottom;

            xe = rborder / 100; //using the variable surfacewidth instead would mess everything up
            ye = bborder / 100;

            // define center point for router
            routerZ = new SKPoint(90 * xe, 50 * ye);
            routerU = new SKPoint(10 * xe, 50 * ye);
            routerV = new SKPoint(35 * xe, 25 * ye);
            routerW = new SKPoint(65 * xe, 25 * ye);
            routerX = new SKPoint(35 * xe, 75 * ye);
            routerY = new SKPoint(65 * xe, 75 * ye);

            //define points for weights
            wUV = new SKPoint(20 * xe, 37.5f * ye);
            wUX = new SKPoint(20 * xe, 65.5f * ye);
            wZW = new SKPoint(80 * xe, 37.5f * ye);
            wZY = new SKPoint(80 * xe, 65.5f * ye);
            wXY = new SKPoint(50 * xe, 23.5f * ye);
            wVW = new SKPoint(50 * xe, 80 * ye);
            wVY = new SKPoint(41.5f * xe, 43.5f * ye);
            wXW = new SKPoint(58.5f * xe, 43.5f * ye);
            wVX = new SKPoint(31.5f * xe, 50 * ye);
            wYW = new SKPoint(68.5f * xe, 50 * ye);
            wUW = new SKPoint(15 * xe, 23.5f * ye);
            wUY = new SKPoint(15 * xe, 80 * ye);
            wZX = new SKPoint(85 * xe, 80 * ye);
            wZV = new SKPoint(85 * xe, 23.5f * ye);
        }

        /**********************************************************************
        *********************************************************************/
        public SKCanvasView ReturnCanvas()
        {
            return this.skiaview;
        }

        /**********************************************************************
        *********************************************************************/
        public static SKCanvasView GetCanvasByID(int id)
        {
            foreach(DijkstraSettingsDraw network in networkList)
            {
                int networkId = network.GetId();
                if(networkId == id)
                {
                    return network.ReturnCanvas();
                }
            }
            return null; //not found
        }

        /**********************************************************************
        *********************************************************************/
        public static DijkstraSettingsDraw GetNetworkByID(int id)
        {
            foreach (DijkstraSettingsDraw network in networkList)
            {
                Debug.WriteLine("thi.id: " + network.GetId());
                int networkId = network.GetId();
                if (networkId == id)
                {
                    Debug.WriteLine("ID FOUND");
                    return network;
                }
            }
            
            Debug.WriteLine("ID NOT FOUND");
            return null; //not found
        }

        /**********************************************************************
        *********************************************************************/
        public int GetId()
        {
            return this.id;
        }

        /**********************************************************************
        *********************************************************************/
        //redraws the canvas
        public void Paint()
        {
            // update the canvas when the data changes
            this.skiaview.InvalidateSurface();

        }

        /**********************************************************************
        *********************************************************************/
        static private void MakeSKPaint()
        {
            //For Text
            sk_blackText = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye * textSize / 1.8f,
                IsAntialias = true,
                IsStroke = false,
                TextAlign = SKTextAlign.Center,
                IsVerticalText = false
            };

            sk_test = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(67, 110, 238).WithAlpha(30)
            };

            sk_RouterText = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.StrokeAndFill,
                TextSize = 35,
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
                Color = SKColors.BlueViolet,
                Style = SKPaintStyle.StrokeAndFill,
                TextSize = 55,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center,
                IsVerticalText = false
            };
        }

    }
}
