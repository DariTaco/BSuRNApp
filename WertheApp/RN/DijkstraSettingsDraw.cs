using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Diagnostics; //Debug.WriteLine("");
using System.Collections.Generic;
using System.Text.RegularExpressions; //Regex.IsMatch

namespace WertheApp.RN
{
    public class DijkstraSettingsDraw
    {
        //VARIABLES
        private SKPoint _touchPoint;
        private SKCanvasView skiaview;
        private SKCanvas canvas;
        private int id;
        private static DijkstraSettings dS;
        private String weightUV, weightUX, weightUW,
        weightUY, weightZW, weightZY,
        weightZV, weightZX, weightVX,
        weightVY, weightVW, weightXW,
        weightXY, weightYW;
        private String action;

        private static List<DijkstraSettingsDraw> networkList = new List<DijkstraSettingsDraw>();
        private static float xe, ye;
        private static SKPaint sk_WeightsText,
            sk_RouterText, sk_RouterContour, sk_RouterFill, sk_test;
        private static float textSize;
        private static SKPoint routerZ, routerU, routerV, routerX, routerW, routerY;
        private static SKPoint wUV, wUX, wUW,
            wUY, wZW, wZY,
            wZV, wZX, wVX,
            wVY, wVW, wXW,
            wXY, wYW;
        private static SKRect rect_wUV, rect_wUX, rect_wUW,
        rect_wUY, rect_wZW, rect_wZY,
        rect_wZV, rect_wZX, rect_wVX,
        rect_wVY, rect_wVW, rect_wXW,
        rect_wXY, rect_wYW;


        //CONSTRUCTOR
        public DijkstraSettingsDraw(int id, DijkstraSettings dijkstraSettings)
        {
            this.id = id;
            networkList.Add(this);
            dS = dijkstraSettings; //TODO

            // crate the canvas
            this.skiaview = new SKCanvasView();
            this.skiaview.PaintSurface += PaintSurface;
            this.skiaview.Touch += OnTouch;
            skiaview.EnableTouchEvents = true;

            textSize = 5;

            SetDefaultWeights();
           
        }

        //METHODS
        public String[] GetAllWeights()
        {
            String[] a = {weightUV, weightUX, weightUW,
        weightUY, weightZW, weightZY,
        weightZV, weightZX, weightVX,
        weightVY, weightVW, weightXW,
        weightXY, weightYW };

            return a;
        }

        /**********************************************************************
        *********************************************************************/
        private async void OnTouch(object sender, SKTouchEventArgs e)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    
                    if (rect_wUV.Contains(e.Location))
                    {
                        //Debug.WriteLine("UV clicked");
                        await dS.OpenPickerPopUp();
                        SetWeight(ref weightUV, action);
                    }
                    else if (rect_wUX.Contains(e.Location))
                    {
                        //Debug.WriteLine("UX clicked");
                        await dS.OpenPickerPopUp();
                        SetWeight(ref weightUX, action);
                    }
                    else if (rect_wUW.Contains(e.Location)
                        && ( this.id == 2 || this.id == 4))
                    {
                        //Debug.WriteLine("UW clicked");
                        await dS.OpenPickerPopUp();
                        SetWeight(ref weightUW, action);
                    }
                    else if (rect_wUY.Contains(e.Location)
                        && (this.id == 3 || this.id == 4))
                    {
                        //Debug.WriteLine("UY clicked");
                        await dS.OpenPickerPopUp();
                        SetWeight(ref weightUY, action);
                    }
                    else if (rect_wZW.Contains(e.Location))
                    {
                        //Debug.WriteLine("ZW clicked");
                        await dS.OpenPickerPopUp();
                        SetWeight(ref weightZW, action);
                    }
                    else if (rect_wZY.Contains(e.Location))
                    {
                        //Debug.WriteLine("ZY clicked");
                        await dS.OpenPickerPopUp();
                        SetWeight(ref weightZY, action);
                    }
                    else if (rect_wZV.Contains(e.Location) && this.id == 4)
                    {
                        //Debug.WriteLine("ZV clicked");
                        await dS.OpenPickerPopUp();
                        SetWeight(ref weightZV, action);
                    }
                    else if (rect_wZX.Contains(e.Location) && this.id == 4)
                    {
                        //Debug.WriteLine("ZX clicked");
                        await dS.OpenPickerPopUp();
                        SetWeight(ref weightZX, action);
                    }
                    else if (rect_wVX.Contains(e.Location)
                        && (this.id == 2 || this.id == 3 || this.id == 4))
                    {
                        //Debug.WriteLine("VX clicked");
                        await dS.OpenPickerPopUp();
                        SetWeight(ref weightVX, action);
                    }
                    else if (rect_wVY.Contains(e.Location) && this.id == 1)
                    {
                        //Debug.WriteLine("VY clicked");
                        await dS.OpenPickerPopUp();
                        SetWeight(ref weightVY, action);
                    }
                    else if (rect_wVW.Contains(e.Location))
                    {
                        //Debug.WriteLine("VW clicked");
                        await dS.OpenPickerPopUp();
                        SetWeight(ref weightVW, action);
                    }
                    else if (rect_wXW.Contains(e.Location)
                        && (this.id == 1 || this.id == 2 || this.id == 3))
                    {
                        //Debug.WriteLine("XW clicked");
                        await dS.OpenPickerPopUp();
                        SetWeight(ref weightXW, action);
                    }
                    else if (rect_wXY.Contains(e.Location))
                    {
                        //Debug.WriteLine("XY clicked");
                        await dS.OpenPickerPopUp();
                        SetWeight(ref weightXY, action);
                    }
                    else if (rect_wYW.Contains(e.Location)
                        && (this.id == 2 || this.id == 3 || this.id == 4))
                    {
                        //Debug.WriteLine("YW clicked");
                        await dS.OpenPickerPopUp();
                        SetWeight(ref weightYW, action);
                    }
                    break;
            }

            e.Handled = true;
        }

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

            //draw Network
            DrawConnections();
            DrawRouters();
            DrawWeights();

            //execute all drawing actions
            canvas.Flush();
        }

        /**********************************************************************
        *********************************************************************/
        public void DrawWeights()
        {
            switch (this.id)
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
        public void DrawConnections()
        {
            switch (this.id)
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
        void DrawWeightsNetwork1()
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
            this.canvas.DrawText(weightUV, wUV, sk_WeightsText);
            this.canvas.DrawText(weightUX, wUX, sk_WeightsText);
            this.canvas.DrawText(weightZW, wZW, sk_WeightsText);
            this.canvas.DrawText(weightZY, wZY, sk_WeightsText);
            this.canvas.DrawText(weightXY, wXY, sk_WeightsText);
            this.canvas.DrawText(weightVW, wVW, sk_WeightsText);
            this.canvas.DrawText(weightXW, wXW, sk_WeightsText);
            this.canvas.DrawText(weightVX, wVX, sk_WeightsText);
            this.canvas.DrawText(weightYW, wYW, sk_WeightsText);
            this.canvas.DrawText(weightUW, wUW, sk_WeightsText);
        }

        /**********************************************************************
        *********************************************************************/
        void DrawWeightsNetwork3()
        {
            this.canvas.DrawText(weightUV, wUV, sk_WeightsText);
            this.canvas.DrawText(weightUX, wUX, sk_WeightsText);
            this.canvas.DrawText(weightZW, wZW, sk_WeightsText);
            this.canvas.DrawText(weightZY, wZY, sk_WeightsText);
            this.canvas.DrawText(weightXY, wXY, sk_WeightsText);
            this.canvas.DrawText(weightVW, wVW, sk_WeightsText);
            this.canvas.DrawText(weightXW, wXW, sk_WeightsText);
            this.canvas.DrawText(weightVX, wVX, sk_WeightsText);
            this.canvas.DrawText(weightYW, wYW, sk_WeightsText);
            this.canvas.DrawText(weightUY, wUY, sk_WeightsText);
        }

        /**********************************************************************
        *********************************************************************/
        void DrawWeightsNetwork4()
        {
            this.canvas.DrawText(weightUV, wUV, sk_WeightsText);
            this.canvas.DrawText(weightUX, wUX, sk_WeightsText);
            this.canvas.DrawText(weightZW, wZW, sk_WeightsText);
            this.canvas.DrawText(weightZY, wZY, sk_WeightsText);
            this.canvas.DrawText(weightXY, wXY, sk_WeightsText);
            this.canvas.DrawText(weightVW, wVW, sk_WeightsText);
            this.canvas.DrawText(weightVX, wVX, sk_WeightsText);
            this.canvas.DrawText(weightYW, wYW, sk_WeightsText);
            this.canvas.DrawText(weightUW, wUW, sk_WeightsText);
            this.canvas.DrawText(weightUY, wUY, sk_WeightsText);
            this.canvas.DrawText(weightZV, wZV, sk_WeightsText);
            this.canvas.DrawText(weightZX, wZX, sk_WeightsText);

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
            SKPoint p = new SKPoint(15 * xe, -30 * ye);
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
            SKPoint p = new SKPoint(15 * xe, 130 * ye);
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

            SKPoint p = new SKPoint(15 * xe, -30 * ye);
            SKPath curveUW = new SKPath();
            curveUW.MoveTo(routerU);
            curveUW.CubicTo(routerU, p, routerW);
            canvas.DrawPath(curveUW, sk_RouterContour);

            SKPoint p2 = new SKPoint(15 * xe, 130 * ye);
            SKPath curveUY = new SKPath();
            curveUY.MoveTo(routerU);
            curveUY.CubicTo(routerU, p2, routerY);
            canvas.DrawPath(curveUY, sk_RouterContour);

            SKPoint p3 = new SKPoint(85 * xe, -30 * ye);
            SKPath curveZV = new SKPath();
            curveZV.MoveTo(routerZ);
            curveZV.CubicTo(routerZ, p3, routerV);
            canvas.DrawPath(curveZV, sk_RouterContour);

            SKPoint p4 = new SKPoint(85 * xe, 130 * ye);
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
            switch (name)
            {
                case "U": this.canvas.DrawText(name, router.X + 90, router.Y + 20, sk_RouterText);
                    break;
                case "V":
                    this.canvas.DrawText(name, router.X, router.Y - 75, sk_RouterText);
                    break;
                case "W":
                    this.canvas.DrawText(name, router.X, router.Y - 75, sk_RouterText);
                    break;
                case "X":
                    this.canvas.DrawText(name, router.X, router.Y + 115, sk_RouterText);
                    break;
                case "Y":
                    this.canvas.DrawText(name, router.X, router.Y + 115, sk_RouterText);
                    break;
                case "Z":
                    this.canvas.DrawText(name, router.X - 90, router.Y + 20, sk_RouterText);
                    break;

            }
            

        }

        /**********************************************************************
        *********************************************************************/
        void DrawConnection(SKPoint a, SKPoint b)
        {
            this.canvas.DrawLine(a, b, sk_RouterContour);
        }

        /**********************************************************************
        *********************************************************************/
        private void SetWeight(ref String o, String w)
        {

            if (w !=  null && Regex.IsMatch(w, "[1-9]"))
            {
               
                o = w;
                this.Paint();
            }

        }

        /**********************************************************************
        *********************************************************************/
        public void SetAction(String a)
        {
            this.action = a;
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
            wUX = new SKPoint(20 * xe, 67.5f * ye);
            wZW = new SKPoint(80 * xe, 37.5f * ye);
            wZY = new SKPoint(80 * xe, 67.5f * ye);
            wVW = new SKPoint(50 * xe, 23.5f * ye);
            wXY = new SKPoint(50 * xe, 80.5f * ye);
            wVY = new SKPoint(41.5f * xe, 43.5f * ye);
            wXW = new SKPoint(58.5f * xe, 43.5f * ye);
            wVX = new SKPoint(31.5f * xe, 50 * ye);
            wYW = new SKPoint(68.5f * xe, 50 * ye);
            wUW = new SKPoint(10 * xe, 23.5f * ye);
            wUY = new SKPoint(10 * xe, 80 * ye);
            wZX = new SKPoint(90 * xe, 80 * ye);
            wZV = new SKPoint(90 * xe, 23.5f * ye);

            //define rectangles for touch
            rect_wUV = new SKRect(wUV.X - 50, wUV.Y - 70, wUV.X + 50, wUV.Y + 30);
            rect_wUX = new SKRect(wUX.X - 50, wUX.Y - 70, wUX.X + 50, wUX.Y + 30);
            rect_wUW = new SKRect(wUW.X - 50, wUW.Y - 70, wUW.X + 50, wUW.Y + 30);
            rect_wUY = new SKRect(wUY.X - 50, wUY.Y - 70, wUY.X + 50, wUY.Y + 30);
            rect_wZW = new SKRect(wZW.X - 50, wZW.Y - 70, wZW.X + 50, wZW.Y + 30);
            rect_wZY = new SKRect(wZY.X - 50, wZY.Y - 70, wZY.X + 50, wZY.Y + 30);
            rect_wZV = new SKRect(wZV.X - 50, wZV.Y - 70, wZV.X + 50, wZV.Y + 30);
            rect_wZX = new SKRect(wZX.X - 50, wZX.Y - 70, wZX.X + 50, wZX.Y + 30);
            rect_wVX = new SKRect(wVX.X - 50, wVX.Y - 70, wVX.X + 50, wVX.Y + 30);
            rect_wVY = new SKRect(wVY.X - 50, wVY.Y - 70, wVY.X + 50, wVY.Y + 30);
            rect_wVW = new SKRect(wVW.X - 50, wVW.Y - 70, wVW.X + 50, wVW.Y + 30);
            rect_wXW = new SKRect(wXW.X - 50, wXW.Y - 70, wXW.X + 50, wXW.Y + 30);
            rect_wXY = new SKRect(wXY.X - 50, wXY.Y - 70, wXY.X + 50, wXY.Y + 30);
            rect_wYW = new SKRect(wYW.X - 50, wYW.Y - 70, wYW.X + 50, wYW.Y + 30);
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
        public static void ClearNetworkList()
        {
            networkList.Clear();
        }
        /**********************************************************************
        *********************************************************************/
        public static DijkstraSettingsDraw GetNetworkByID(int id)
        {
            int count = 0;
            foreach (DijkstraSettingsDraw network in networkList)
            {
                count++;
          
                int networkId = network.GetId();
                if (networkId == id)
                {
                    
                    return network;
                }
            }
            
            
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
        public void SetDefaultWeights()
        {
            switch (this.id)
            {
                case 1:
                    weightUV = "1";
                    weightUX = "1";
                    weightZW = "1";
                    weightZY = "1";
                    weightXY = "1";
                    weightVW = "1";

                    weightVY = "1";
                    weightXW = "1";
                    weightVX = "0";
                    weightYW = "0";
                    weightUW = "0";
                    weightUY = "0";

                    weightZV = "0";
                    weightZX = "0";
                    break;
                case 2:
                    weightUV = "1";
                    weightUX = "1";
                    weightZW = "1";
                    weightZY = "1";
                    weightXY = "1";
                    weightVW = "1";

                    weightVY = "0";
                    weightXW = "1";
                    weightVX = "1";
                    weightYW = "1";
                    weightUW = "1";
                    weightUY = "0";

                    weightZV = "0";
                    weightZX = "0";
                    break;
                case 3:
                    weightUV = "1";
                    weightUX = "1";
                    weightZW = "1";
                    weightZY = "1";
                    weightXY = "1";
                    weightVW = "1";

                    weightVY = "0";
                    weightXW = "1";
                    weightVX = "1";
                    weightYW = "1";
                    weightUW = "0";
                    weightUY = "1";

                    weightZV = "0";
                    weightZX = "0";
                    break;
                case 4:
                    weightUV = "1";
                    weightUX = "1";
                    weightZW = "1";
                    weightZY = "1";
                    weightXY = "1";
                    weightVW = "1";

                    weightVY = "0";
                    weightXW = "0";
                    weightVX = "1";
                    weightYW = "1";
                    weightUW = "1";
                    weightUY = "1";

                    weightZV = "1";
                    weightZX = "1";
                    break;
            }
            this.Paint();
        }


        /**********************************************************************
        *********************************************************************/
        public void SetRandomWeights()
        {
            Random rnd = new Random();
            switch (this.id)
            {
                case 1:
                    weightUV = rnd.Next(1,9).ToString();
                    weightUX = rnd.Next(1, 9).ToString();
                    weightZW = rnd.Next(1, 9).ToString();
                    weightZY = rnd.Next(1, 9).ToString();
                    weightXY = rnd.Next(1, 9).ToString();
                    weightVW = rnd.Next(1, 9).ToString();
                    weightVY = rnd.Next(1, 9).ToString();
                    weightXW = rnd.Next(1, 9).ToString();
                    break;
                case 2:
                    weightUV = rnd.Next(1, 9).ToString();
                    weightUX = rnd.Next(1, 9).ToString();
                    weightZW = rnd.Next(1, 9).ToString();
                    weightZY = rnd.Next(1, 9).ToString();
                    weightXY = rnd.Next(1, 9).ToString();
                    weightVW = rnd.Next(1, 9).ToString();
                    weightXW = rnd.Next(1, 9).ToString();
                    weightVX = rnd.Next(1, 9).ToString();
                    weightYW = rnd.Next(1, 9).ToString();
                    weightUW = rnd.Next(1, 9).ToString();
                    break;
                case 3:
                    weightUV = rnd.Next(1, 9).ToString();
                    weightUX = rnd.Next(1, 9).ToString();
                    weightZW = rnd.Next(1, 9).ToString();
                    weightZY = rnd.Next(1, 9).ToString();
                    weightXY = rnd.Next(1, 9).ToString();
                    weightVW = rnd.Next(1, 9).ToString();
                    weightXW = rnd.Next(1, 9).ToString();
                    weightVX = rnd.Next(1, 9).ToString();
                    weightYW = rnd.Next(1, 9).ToString();
                    weightUY = rnd.Next(1, 9).ToString();
                    break;
                case 4:
                    weightUV = rnd.Next(1, 9).ToString();
                    weightUX = rnd.Next(1, 9).ToString();
                    weightZW = rnd.Next(1, 9).ToString();
                    weightZY = rnd.Next(1, 9).ToString();
                    weightXY = rnd.Next(1, 9).ToString();
                    weightVW = rnd.Next(1, 9).ToString();
                    weightVX = rnd.Next(1, 9).ToString();
                    weightYW = rnd.Next(1, 9).ToString();
                    weightUW = rnd.Next(1, 9).ToString();
                    weightUY = rnd.Next(1, 9).ToString();
                    weightZV = rnd.Next(1, 9).ToString();
                    weightZX = rnd.Next(1, 9).ToString();
                    break;
            }
            this.Paint();
        }

        /**********************************************************************
        *********************************************************************/
        public void SetPresetsWeights()
        {
            switch (this.id)
            {
                case 1:
                    weightUV = "2";
                    weightUX = "1";
                    weightZW = "3";
                    weightZY = "5";
                    weightXY = "2";
                    weightVW = "7";
                    weightVY = "1";
                    weightXW = "9";
                    break;
                case 2:
                    weightUV = "7";
                    weightUX = "6";
                    weightZW = "1";
                    weightZY = "5";
                    weightXY = "8";
                    weightVW = "7";
                    weightXW = "2";
                    weightVX = "3";
                    weightYW = "2";
                    weightUW = "4";
                    break;
                case 3:
                    weightUV = "1";
                    weightUX = "1";
                    weightZW = "2";
                    weightZY = "1";
                    weightXY = "2";
                    weightVW = "1";
                    weightXW = "1";
                    weightVX = "6";
                    weightYW = "1";
                    weightUY = "3";
                    break;
                case 4:
                    weightUV = "7";
                    weightUX = "4";
                    weightZW = "7";
                    weightZY = "8";
                    weightXY = "3";
                    weightVW = "2";
                    weightVX = "1";
                    weightYW = "6";
                    weightUW = "6";
                    weightUY = "5";
                    weightZV = "5";
                    weightZX = "5";
                    break;
            }
            this.Paint();
        }

        /**********************************************************************
        *********************************************************************/
        static private void MakeSKPaint()
        {
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
                TextSize = 55,
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
