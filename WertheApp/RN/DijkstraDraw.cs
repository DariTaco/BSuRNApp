using System;
using System.Diagnostics;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Linq;

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
        private static int maxStep;

        private static SKPaint sk_Background, sk_PaintBlack, sk_RouterText,
        sk_RouterContour, sk_RouterFill, sk_RouterFillRed, sk_WeightsText,
            sk_TableCaption, sk_TableCaptionRed, sk_Visited,
            sk_PaintBlackThin, sk_Background2;
        private static float strokeWidth;


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
            DijkstraAlgorithm.CreateVisitedEdges();

            // crate the canvas
            skiaview = new SKCanvasView();
            skiaview.PaintSurface += PaintSurface;

            
            strokeWidth = 0.2f;

            currentStep = 0;
            maxStep = 31;
            
           
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

            DrawTableContents();
            if(currentStep == maxStep)
            {
                DrawForwardingTable();
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
            SKRect sk_rBackground2 = new SKRect(00 * xe, 178 * ye2, 100 * xe, 200 * ye2); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground2, sk_Background2); //left, top, right, bottom, color
            canvas.DrawLine(new SKPoint(0 * xe, 100 * ye2), new SKPoint(100 * xe, 100 * ye2), sk_PaintBlack);

        }
        /**********************************************************************
        *********************************************************************/
        public static void DrawForwardingTable()
        {
            String[] a = DijkstraAlgorithm.GetForwardingTable();
            canvas.DrawText(a[0], new SKPoint(34 * xe, 195.2f * ye2), sk_TableCaption);
            canvas.DrawText(a[1], new SKPoint(48 * xe, 195.2f * ye2), sk_TableCaption);
            canvas.DrawText(a[2], new SKPoint(62 * xe, 195.2f * ye2), sk_TableCaption);
            canvas.DrawText(a[3], new SKPoint(76 * xe, 195.2f * ye2), sk_TableCaption);
            canvas.DrawText(a[4], new SKPoint(90 * xe, 195.2f * ye2), sk_TableCaption);
        }

        /**********************************************************************
        *********************************************************************/
        public static void DrawTableContents()
        {
            String[,] tableContents = DijkstraAlgorithm.GetTableValues();

            String visitedNodes = tableContents[currentStep, 8];
        

            for (int i = 0; i <= currentStep; i++)
            {
                // check which is the current table row and write text in this row
                int round = Int32.Parse(tableContents[i, 1]);

                float yValueContent = 0;
                switch (round)
                {
                    case 0: yValueContent = 123.2f; break;
                    case 1: yValueContent = 133.2f; break;
                    case 2: yValueContent = 143.2f; break;
                    case 3: yValueContent = 153.2f; break;
                    case 4: yValueContent = 163.2f; break;
                    case 5: yValueContent = 173.2f; break;
                }

                
                canvas.DrawText(tableContents[i, 2], new SKPoint(20 * xe, yValueContent * ye2), sk_TableCaption);
                canvas.DrawText(tableContents[i, 3], new SKPoint(34 * xe, yValueContent * ye2), sk_TableCaption);
                canvas.DrawText(tableContents[i, 4], new SKPoint(48 * xe, yValueContent * ye2), sk_TableCaption);
                canvas.DrawText(tableContents[i, 5], new SKPoint(62 * xe, yValueContent * ye2), sk_TableCaption);
                canvas.DrawText(tableContents[i, 6], new SKPoint(76 * xe, yValueContent * ye2), sk_TableCaption);
                canvas.DrawText(tableContents[i, 7], new SKPoint(90 * xe, yValueContent * ye2), sk_TableCaption);

                
                char visitedNodeToMark = visitedNodes[round];
                yValueContent = yValueContent - 10f; //previous row/round

                // every new round/ every 7th step
                if (i % 6 == 0)
                {
                    //overdraw the previously visited connection in red
                    switch (visitedNodeToMark)
                    {
                        // v = 1st step previous row (i-5) , w = 2nd step..., x = 3rd step..., y = 4th step..., z = 5th step...
                        case 'v':
                            canvas.DrawText(tableContents[i-5, 3], new SKPoint(34 * xe, yValueContent * ye2), sk_TableCaptionRed);
                            break;
                        case 'w':
                            canvas.DrawText(tableContents[i-4, 4], new SKPoint(48 * xe, yValueContent * ye2), sk_TableCaptionRed);
                            break;
                        case 'x':
                            canvas.DrawText(tableContents[i-3, 5], new SKPoint(62 * xe, yValueContent * ye2), sk_TableCaptionRed);
                            break;
                        case 'y':
                            canvas.DrawText(tableContents[i-2, 6], new SKPoint(76 * xe, yValueContent * ye2), sk_TableCaptionRed);
                            break;
                        case 'z':
                            canvas.DrawText(tableContents[i-1, 7], new SKPoint(90 * xe, yValueContent * ye2), sk_TableCaptionRed);
                            break;
                    }
                }
                    


            }
        }

        /**********************************************************************
        *********************************************************************/
        public static void DrawTable()
        {

            float cap = 113f;
            float cap2 = 107.5f;
            canvas.DrawText("Step", new SKPoint(5 * xe, cap * ye2), sk_TableCaption);
            canvas.DrawText("N'", new SKPoint(20 * xe, cap * ye2), sk_TableCaption);
            canvas.DrawText("D(v),", new SKPoint(34 * xe, cap2 * ye2), sk_TableCaption);
            canvas.DrawText("D(w),", new SKPoint(48 * xe, cap2 * ye2), sk_TableCaption);
            canvas.DrawText("D(x),", new SKPoint(62 * xe, cap2 * ye2), sk_TableCaption);
            canvas.DrawText("D(y),", new SKPoint(76 * xe, cap2 * ye2), sk_TableCaption);
            canvas.DrawText("D(z),", new SKPoint(90 * xe, cap2 * ye2), sk_TableCaption);
            canvas.DrawText("p(v)", new SKPoint(34 * xe, cap * ye2), sk_TableCaption);
            canvas.DrawText("p(w)", new SKPoint(48 * xe, cap * ye2), sk_TableCaption);
            canvas.DrawText("p(x)", new SKPoint(62 * xe, cap * ye2), sk_TableCaption);
            canvas.DrawText("p(y)", new SKPoint(76 * xe, cap * ye2), sk_TableCaption);
            canvas.DrawText("p(z)", new SKPoint(90 * xe, cap * ye2), sk_TableCaption);

            canvas.DrawLine(new SKPoint(1 * xe, 116 * ye2), new SKPoint(99 * xe, 116 * ye2), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(1 * xe, 126 * ye2), new SKPoint(99 * xe, 126 * ye2), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(1 * xe, 136 * ye2), new SKPoint(99 * xe, 136 * ye2), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(1 * xe, 146 * ye2), new SKPoint(99 * xe, 146 * ye2), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(1 * xe, 156 * ye2), new SKPoint(99 * xe, 156 * ye2), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(1 * xe, 166 * ye2), new SKPoint(99 * xe, 166 * ye2), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(0 * xe, 178 * ye2), new SKPoint(100 * xe, 178 * ye2), sk_PaintBlack);


            canvas.DrawText("Destination", new SKPoint(6f * xe + 50, 186.2f * ye2), sk_TableCaption);
            canvas.DrawText("v", new SKPoint(34 * xe, 186.2f * ye2), sk_TableCaption);
            canvas.DrawText("w", new SKPoint(48 * xe, 186.2f * ye2), sk_TableCaption);
            canvas.DrawText("x", new SKPoint(62 * xe, 186.2f * ye2), sk_TableCaption);
            canvas.DrawText("y", new SKPoint(76 * xe, 186.2f * ye2), sk_TableCaption);
            canvas.DrawText("z", new SKPoint(90 * xe, 186.2f * ye2), sk_TableCaption);
            canvas.DrawText("Link", new SKPoint(5 * xe + 30, 195.2f * ye2), sk_TableCaption);
            canvas.DrawLine(new SKPoint(1 * xe, 189 * ye2), new SKPoint(99 * xe, 189 * ye2), sk_PaintBlackThin);

            canvas.DrawText("0", new SKPoint(5 * xe, 123.2f * ye2), sk_TableCaption);
            canvas.DrawText("1", new SKPoint(5 * xe, 133.2f * ye2), sk_TableCaption);
            canvas.DrawText("2", new SKPoint(5 * xe, 143.2f * ye2), sk_TableCaption);
            canvas.DrawText("3", new SKPoint(5 * xe, 153.2f * ye2), sk_TableCaption);
            canvas.DrawText("4", new SKPoint(5 * xe, 163.2f * ye2), sk_TableCaption);
            canvas.DrawText("5", new SKPoint(5 * xe, 173.2f * ye2), sk_TableCaption);

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
            String[,] tableContents = DijkstraAlgorithm.GetTableValues();
            String visitedNodes = tableContents[currentStep, 8];

            DrawRouter(routerZ, "Z", visitedNodes.Contains('z'));
            DrawRouter(routerU, "U", visitedNodes.Contains('u'));
            DrawRouter(routerV, "V", visitedNodes.Contains('v'));
            DrawRouter(routerW, "W", visitedNodes.Contains('w'));
            DrawRouter(routerX, "X", visitedNodes.Contains('x'));
            DrawRouter(routerY, "Y", visitedNodes.Contains('y'));
        }

        /**********************************************************************
        *********************************************************************/
        void DrawRouter(SKPoint router, String name, bool visited)
        {
            float radius = 40;

            //router

            if (visited)
            {
                canvas.DrawCircle(router, radius, sk_RouterFillRed);
                canvas.DrawCircle(router, radius, sk_Visited);
            }
            else
            {
                canvas.DrawCircle(router, radius, sk_RouterFill);
                canvas.DrawCircle(router, radius, sk_RouterContour);
            }
           

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
            bool[,] visitedEgdes = DijkstraAlgorithm.GetVisistedEdges();
            DrawConnection(routerX, routerU, visitedEgdes[currentStep, 1]);
            DrawConnection(routerX, routerW, visitedEgdes[currentStep, 11]);
            DrawConnection(routerX, routerY, visitedEgdes[currentStep, 12]);
            DrawConnection(routerU, routerV, visitedEgdes[currentStep, 0]);
            //DrawConnections(canvas, routerV, routerX);
            DrawConnection(routerV, routerW, visitedEgdes[currentStep, 10]);
            DrawConnection(routerV, routerY, visitedEgdes[currentStep, 9]);
            //DrawConnections(canvas, routerW, routerY);
            DrawConnection(routerW, routerZ, visitedEgdes[currentStep, 4]);
            DrawConnection(routerZ, routerY, visitedEgdes[currentStep, 5]);
        }

        /**********************************************************************
        *********************************************************************/
        void DrawConnectionsNetwork2()
        {
            bool[,] visitedEgdes = DijkstraAlgorithm.GetVisistedEdges();
            DrawConnection(routerX, routerU, visitedEgdes[currentStep, 1]);
            DrawConnection(routerX, routerW, visitedEgdes[currentStep, 11]);
            DrawConnection(routerX, routerY, visitedEgdes[currentStep, 12]);
            DrawConnection(routerU, routerV, visitedEgdes[currentStep, 0]);
            DrawConnection(routerV, routerX, visitedEgdes[currentStep, 8]);
            DrawConnection(routerV, routerW, visitedEgdes[currentStep, 10]);
            //DrawConnections(canvas, routerV, routerY);
            DrawConnection(routerW, routerY, visitedEgdes[currentStep, 13]);
            DrawConnection(routerW, routerZ, visitedEgdes[currentStep, 4]);
            DrawConnection(routerZ, routerY, visitedEgdes[currentStep, 5]);
            SKPoint p = new SKPoint(15 * xe, -15 * ye2);
            SKPath curveUW = new SKPath();
            curveUW.MoveTo(routerU);
            curveUW.CubicTo(routerU, p, routerW);
            if(visitedEgdes[currentStep, 2]){ canvas.DrawPath(curveUW, sk_Visited); }
            else{ if (currentStep != maxStep) { canvas.DrawPath(curveUW, sk_RouterContour); } }

        }

        /**********************************************************************
        *********************************************************************/
        void DrawConnectionsNetwork3()
        {
            bool[,] visitedEgdes = DijkstraAlgorithm.GetVisistedEdges();
            DrawConnection(routerX, routerU, visitedEgdes[currentStep, 1]);
            DrawConnection(routerX, routerW, visitedEgdes[currentStep, 11]);
            DrawConnection(routerX, routerY, visitedEgdes[currentStep, 12]);
            DrawConnection(routerU, routerV, visitedEgdes[currentStep, 0]);
            DrawConnection(routerV, routerX, visitedEgdes[currentStep, 8]);
            DrawConnection(routerV, routerW, visitedEgdes[currentStep, 10]);
            //DrawConnections(canvas, routerV, routerY);
            DrawConnection(routerW, routerY, visitedEgdes[currentStep, 13]);
            DrawConnection(routerW, routerZ, visitedEgdes[currentStep, 4]);
            DrawConnection(routerZ, routerY, visitedEgdes[currentStep, 5]);
            SKPoint p = new SKPoint(15 * xe, 115 * ye2);
            SKPath curveUY = new SKPath();
            curveUY.MoveTo(routerU);
            curveUY.CubicTo(routerU, p, routerY);
            if(visitedEgdes[currentStep, 3]) { canvas.DrawPath(curveUY, sk_Visited); }
            else{ if (currentStep != maxStep) { canvas.DrawPath(curveUY, sk_RouterContour); } }
        }

        /**********************************************************************
        *********************************************************************/
        void DrawConnectionsNetwork4()
        {
            bool[,] visitedEgdes = DijkstraAlgorithm.GetVisistedEdges();
            DrawConnection(routerX, routerU, visitedEgdes[currentStep, 1]);
            //DrawConnections(canvas, routerX, routerW);
            DrawConnection(routerX, routerY, visitedEgdes[currentStep, 12]);
            DrawConnection(routerU, routerV, visitedEgdes[currentStep, 0]);
            DrawConnection(routerV, routerX, visitedEgdes[currentStep, 8]);
            DrawConnection(routerV, routerW, visitedEgdes[currentStep, 10]);
            //DrawConnections(canvas, routerV, routerY);
            DrawConnection(routerW, routerY, visitedEgdes[currentStep, 13]);
            DrawConnection(routerW, routerZ, visitedEgdes[currentStep, 4]);
            DrawConnection(routerZ, routerY, visitedEgdes[currentStep, 5]);

            SKPoint p = new SKPoint(15 * xe, -15 * ye2);
            SKPath curveUW = new SKPath();
            curveUW.MoveTo(routerU);
            curveUW.CubicTo(routerU, p, routerW);
            if (visitedEgdes[currentStep, 2]) { canvas.DrawPath(curveUW, sk_Visited); }
            else { if (currentStep != maxStep) { canvas.DrawPath(curveUW, sk_RouterContour); } }

            SKPoint p2 = new SKPoint(15 * xe, 115 * ye2);
            SKPath curveUY = new SKPath();
            curveUY.MoveTo(routerU);
            curveUY.CubicTo(routerU, p2, routerY);
            if (visitedEgdes[currentStep, 3]) { canvas.DrawPath(curveUY, sk_Visited); }
            else { if (currentStep != maxStep) { canvas.DrawPath(curveUY, sk_RouterContour); } }

            SKPoint p3 = new SKPoint(85 * xe, -15 * ye2);
            SKPath curveZV = new SKPath();
            curveZV.MoveTo(routerZ);
            curveZV.CubicTo(routerZ, p3, routerV);
            if (visitedEgdes[currentStep, 6]) { canvas.DrawPath(curveZV, sk_Visited); }
            else { if (currentStep != maxStep) { canvas.DrawPath(curveZV, sk_RouterContour); } }
                

            SKPoint p4 = new SKPoint(85 * xe, 115 * ye2);
            SKPath curveZX = new SKPath();
            curveZX.MoveTo(routerZ);
            curveZX.CubicTo(routerZ, p4, routerX);
            if (visitedEgdes[currentStep, 7]) { canvas.DrawPath(curveZX, sk_Visited); }
            else { if (currentStep != maxStep) { canvas.DrawPath(curveZX, sk_RouterContour); } }   
        }

        /**********************************************************************
        *********************************************************************/
        void DrawConnection(SKPoint a, SKPoint b, bool visited)
        {
            if (visited)
            {
                canvas.DrawLine(a, b, sk_Visited);
            }
            else
            {
                if (currentStep != maxStep)
                {
                    canvas.DrawLine(a, b, sk_RouterContour);
                }
                
            }
            
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
            if (currentStep != maxStep)
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

            sk_TableCaptionRed = new SKPaint
            {
                Color = SKColors.Red,
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

            sk_RouterFillRed = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(238, 170, 170)
            };

            sk_RouterContour = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(100, 100, 100)
            };

            sk_Visited = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(240, 10, 50)
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
