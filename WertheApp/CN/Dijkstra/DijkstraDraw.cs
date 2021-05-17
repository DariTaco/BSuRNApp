using System;
using System.Diagnostics;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Linq;

namespace WertheApp.CN
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
        private static int currentStep;
        private static int maxStep;

        private static SKPaint sk_Background, sk_PaintBlack, sk_RouterText,
        sk_RouterContour, sk_RouterFill, sk_RouterFillRed, sk_WeightsText,
            sk_TableCaption, sk_TableCaptionRed, sk_Visited,
            sk_PaintBlackThin, sk_Background2;

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

            
            currentStep = 0;
            maxStep = 31;
            
           
        }


        //METHODS
        // do the drawing
        void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            info = e.Info;
            //canvas object
            SKSurface surface = e.Surface;
            canvas = surface.Canvas;

            //Important! Otherwise the drawing will look messed up in iOS
            if (canvas != null) { canvas.Clear(); }

            //calculate some stuff and make the paint
            CalculateNeededNumbers();
            CalculateNeededVariables();
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
            SKRect sk_rBackground = new SKRect(x1, y1, x2, y2/2); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground, sk_Background); //left, top, right, bottom, color
            SKRect sk_rBackground2 = new SKRect(x1, yPercent(0.89f), x2, y2); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground2, sk_Background2); //left, top, right, bottom, color
            canvas.DrawLine(new SKPoint(x1, y2/2), new SKPoint(x2, y2/2), sk_PaintBlack);

        }
        /**********************************************************************
        *********************************************************************/
        public static void DrawForwardingTable()
        {
            String[] a = DijkstraAlgorithm.GetForwardingTable();
            canvas.DrawText(a[0], new SKPoint(xPercent(0.34f), yPercent(0.985f)), sk_TableCaption);
            canvas.DrawText(a[1], new SKPoint(xPercent(0.48f), yPercent(0.985f)), sk_TableCaption);
            canvas.DrawText(a[2], new SKPoint(xPercent(0.62f), yPercent(0.985f)), sk_TableCaption);
            canvas.DrawText(a[3], new SKPoint(xPercent(0.76f), yPercent(0.985f)), sk_TableCaption);
            canvas.DrawText(a[4], new SKPoint(xPercent(0.9f), yPercent(0.985f)), sk_TableCaption);
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
                    case 0: yValueContent = yPercent(0.615f); break;
                    case 1: yValueContent = yPercent(0.665f); break;
                    case 2: yValueContent = yPercent(0.715f); break;
                    case 3: yValueContent = yPercent(0.765f); break;
                    case 4: yValueContent = yPercent(0.815f); break;
                    case 5: yValueContent = yPercent(0.865f); break;
                }

                canvas.DrawText(tableContents[i, 2], new SKPoint(xPercent(0.2f), yValueContent), sk_TableCaption);
                canvas.DrawText(tableContents[i, 3], new SKPoint(xPercent(0.34f), yValueContent), sk_TableCaption);
                canvas.DrawText(tableContents[i, 4], new SKPoint(xPercent(0.48f), yValueContent), sk_TableCaption);
                canvas.DrawText(tableContents[i, 5], new SKPoint(xPercent(0.62f), yValueContent), sk_TableCaption);
                canvas.DrawText(tableContents[i, 6], new SKPoint(xPercent(0.76f), yValueContent), sk_TableCaption);
                canvas.DrawText(tableContents[i, 7], new SKPoint(xPercent(0.9f), yValueContent), sk_TableCaption);

                
                char visitedNodeToMark = visitedNodes[round];
                yValueContent = yValueContent - yPercent(0.05f); //previous row/round

                // every new round/ every 7th step
                if (i % 6 == 0)
                {
                    //overdraw the previously visited connection in red
                    switch (visitedNodeToMark)
                    {
                        // v = 1st step previous row (i-5) , w = 2nd step..., x = 3rd step..., y = 4th step..., z = 5th step...
                        case 'v':
                            canvas.DrawText(tableContents[i-5, 3], new SKPoint(xPercent(0.34f), yValueContent), sk_TableCaptionRed);
                            break;
                        case 'w':
                            canvas.DrawText(tableContents[i-4, 4], new SKPoint(xPercent(0.48f), yValueContent), sk_TableCaptionRed);
                            break;
                        case 'x':
                            canvas.DrawText(tableContents[i-3, 5], new SKPoint(xPercent(0.62f), yValueContent), sk_TableCaptionRed);
                            break;
                        case 'y':
                            canvas.DrawText(tableContents[i-2, 6], new SKPoint(xPercent(0.76f), yValueContent), sk_TableCaptionRed);
                            break;
                        case 'z':
                            canvas.DrawText(tableContents[i-1, 7], new SKPoint(xPercent(0.9f), yValueContent), sk_TableCaptionRed);
                            break;
                    }
                }
                    


            }
        }

        /**********************************************************************
        *********************************************************************/
        public static void DrawTable()
        {

            float cap = yPercent(0.565f);
            float cap2 = yPercent(0.53f);
            canvas.DrawText("Step", new SKPoint(xPercent(0.05f), cap), sk_TableCaption);
            canvas.DrawText("N'", new SKPoint(xPercent(0.2f), cap), sk_TableCaption);
            canvas.DrawText("D(v),", new SKPoint(xPercent(0.34f), cap2), sk_TableCaption);
            canvas.DrawText("D(w),", new SKPoint(xPercent(0.48f), cap2), sk_TableCaption);
            canvas.DrawText("D(x),", new SKPoint(xPercent(0.62f), cap2), sk_TableCaption);
            canvas.DrawText("D(y),", new SKPoint(xPercent(0.76f), cap2), sk_TableCaption);
            canvas.DrawText("D(z),", new SKPoint(xPercent(0.9f), cap2), sk_TableCaption);
            canvas.DrawText("p(v)", new SKPoint(xPercent(0.34f), cap), sk_TableCaption);
            canvas.DrawText("p(w)", new SKPoint(xPercent(0.48f), cap), sk_TableCaption);
            canvas.DrawText("p(x)", new SKPoint(xPercent(0.62f), cap), sk_TableCaption);
            canvas.DrawText("p(y)", new SKPoint(xPercent(0.76f), cap), sk_TableCaption);
            canvas.DrawText("p(z)", new SKPoint(xPercent(0.9f), cap), sk_TableCaption);

            //TODO
            canvas.DrawLine(new SKPoint(xPercent(0.01f), yPercent(0.58f)), new SKPoint(xPercent(0.99f), yPercent(0.58f)), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(xPercent(0.01f), yPercent(0.63f)), new SKPoint(xPercent(0.99f), yPercent(0.63f)), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(xPercent(0.01f), yPercent(0.68f)), new SKPoint(xPercent(0.99f), yPercent(0.68f)), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(xPercent(0.01f), yPercent(0.73f)), new SKPoint(xPercent(0.99f), yPercent(0.73f)), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(xPercent(0.01f), yPercent(0.78f)), new SKPoint(xPercent(0.99f), yPercent(0.78f)), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(xPercent(0.01f), yPercent(0.83f)), new SKPoint(xPercent(0.99f), yPercent(0.83f)), sk_PaintBlackThin);
            canvas.DrawLine(new SKPoint(x1, yPercent(0.89f)), new SKPoint(x2, yPercent(0.89f)), sk_PaintBlack);


            canvas.DrawText("Destination", new SKPoint(xPercent(0.11f), yPercent(0.93f)), sk_TableCaption);
            canvas.DrawText("v", new SKPoint(xPercent(0.34f), yPercent(0.93f)), sk_TableCaption);
            canvas.DrawText("w", new SKPoint(xPercent(0.48f), yPercent(0.93f)), sk_TableCaption);
            canvas.DrawText("x", new SKPoint(xPercent(0.62f), yPercent(0.93f)), sk_TableCaption);
            canvas.DrawText("y", new SKPoint(xPercent(0.76f), yPercent(0.93f)), sk_TableCaption);
            canvas.DrawText("z", new SKPoint(xPercent(0.9f), yPercent(0.93f)), sk_TableCaption);
            canvas.DrawText("Link", new SKPoint(xPercent(0.07f), yPercent(0.985f)), sk_TableCaption);
            canvas.DrawLine(new SKPoint(xPercent(0.01f), yPercent(0.95f)), new SKPoint(xPercent(0.99f), yPercent(0.95f)), sk_PaintBlackThin);

            canvas.DrawText("0", new SKPoint(xPercent(0.05f), yPercent(0.615f)), sk_TableCaption);
            canvas.DrawText("1", new SKPoint(xPercent(0.05f), yPercent(0.665f)), sk_TableCaption);
            canvas.DrawText("2", new SKPoint(xPercent(0.05f), yPercent(0.715f)), sk_TableCaption);
            canvas.DrawText("3", new SKPoint(xPercent(0.05f), yPercent(0.765f)), sk_TableCaption);
            canvas.DrawText("4", new SKPoint(xPercent(0.05f), yPercent(0.815f)), sk_TableCaption);
            canvas.DrawText("5", new SKPoint(xPercent(0.05f), yPercent(0.865f)), sk_TableCaption);

        }

      
        /**********************************************************************
        *********************************************************************/
        static void CalculateNeededNumbers()
        {
            // define center point for router
            routerZ = new SKPoint(xPercent(0.9f), yPercent(0.25f));
            routerU = new SKPoint(xPercent(0.1f), yPercent(0.25f));
            routerV = new SKPoint(xPercent(0.35f), yPercent(0.125f));
            routerW = new SKPoint(xPercent(0.65f), yPercent(0.125f));
            routerX = new SKPoint(xPercent(0.35f), yPercent(0.375f));
            routerY = new SKPoint(xPercent(0.65f), yPercent(0.375f));

            //define points for weights
            wUV = new SKPoint(xPercent(0.2f), yPercent(0.175f));
            wUX = new SKPoint(xPercent(0.2f), yPercent(0.355f));
            wZW = new SKPoint(xPercent(0.8f), yPercent(0.175f));
            wZY = new SKPoint(xPercent(0.8f), yPercent(0.355f));
            wVW = new SKPoint(xPercent(0.5f), yPercent(0.115f));
            wXY = new SKPoint(xPercent(0.5f), yPercent(0.41f));
            wVY = new SKPoint(xPercent(0.415f), yPercent(0.24f));
            wXW = new SKPoint(xPercent(0.585f), yPercent(0.24f));
            wVX = new SKPoint(xPercent(0.315f), yPercent(0.25f));
            wYW = new SKPoint(xPercent(0.685f), yPercent(0.25f));
            wUW = new SKPoint(xPercent(0.15f), yPercent(0.115f));
            wUY = new SKPoint(xPercent(0.15f), yPercent(0.43f));
            wZX = new SKPoint(xPercent(0.85f), yPercent(0.43f));
            wZV = new SKPoint(xPercent(0.85f), yPercent(0.115f));
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
            float radius = xPercent(0.05f);

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
            canvas.DrawText(name, router.X, router.Y + xPercent(0.02f), sk_RouterText);

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
            SKPoint p = new SKPoint(xPercent(0.15f), -yPercent(0.1f));
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
            SKPoint p = new SKPoint(xPercent(0.15f), yPercent(0.6f));
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

            SKPoint p = new SKPoint(xPercent(0.15f), -yPercent(0.1f));
            SKPath curveUW = new SKPath();
            curveUW.MoveTo(routerU);
            curveUW.CubicTo(routerU, p, routerW);
            if (visitedEgdes[currentStep, 2]) { canvas.DrawPath(curveUW, sk_Visited); }
            else { if (currentStep != maxStep) { canvas.DrawPath(curveUW, sk_RouterContour); } }

            SKPoint p2 = new SKPoint(xPercent(0.15f), yPercent(0.6f));
            SKPath curveUY = new SKPath();
            curveUY.MoveTo(routerU);
            curveUY.CubicTo(routerU, p2, routerY);
            if (visitedEgdes[currentStep, 3]) { canvas.DrawPath(curveUY, sk_Visited); }
            else { if (currentStep != maxStep) { canvas.DrawPath(curveUY, sk_RouterContour); } }

            SKPoint p3 = new SKPoint(xPercent(0.85f), -yPercent(0.1f));
            SKPath curveZV = new SKPath();
            curveZV.MoveTo(routerZ);
            curveZV.CubicTo(routerZ, p3, routerV);
            if (visitedEgdes[currentStep, 6]) { canvas.DrawPath(curveZV, sk_Visited); }
            else { if (currentStep != maxStep) { canvas.DrawPath(curveZV, sk_RouterContour); } }
                

            SKPoint p4 = new SKPoint(xPercent(0.85f), yPercent(0.6f));
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
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                Color = new SKColor(67, 110, 238).WithAlpha(30)
            };

            sk_Background2 = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                Color = new SKColor(238, 130, 238).WithAlpha(30)
            };

            sk_PaintBlackThin = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth /2 ,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(0, 0, 0) //black
            };

            sk_PaintBlack = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * 2,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(0, 0, 0) //black
            };

            sk_RouterText = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.StrokeAndFill,
                TextSize = xPercent(0.05f),
                IsAntialias = true,
                TextAlign = SKTextAlign.Center,
            };

            sk_TableCaption = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.StrokeAndFill,
                TextSize = yPercent(0.025f),
                IsAntialias = true,
                TextAlign = SKTextAlign.Center,
            };

            sk_TableCaptionRed = new SKPaint
            {
                Color = SKColors.Red,
                Style = SKPaintStyle.StrokeAndFill,
                TextSize = yPercent(0.025f),
                IsAntialias = true,
                TextAlign = SKTextAlign.Center,
            };

            sk_RouterFill = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                Color = new SKColor(170, 170, 170)
            };

            sk_RouterFillRed = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                Color = new SKColor(238, 170, 170)
            };

            sk_RouterContour = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                Color = new SKColor(100, 100, 100)
            };

            sk_Visited = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                Color = new SKColor(240, 10, 50)
            };

            sk_WeightsText = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.StrokeAndFill,
                TextSize = xPercent(0.04f),
                IsAntialias = true,
                TextAlign = SKTextAlign.Center,
            };
        }
        // canvas
        private static SKImageInfo info; // canvas info
        private static float centerX, centerY, x1, x2, y1, y2; // canvas coordinates

        // painting tools
        private static float strokeWidth; // stroke Width for paint colors
        private static void CalculateNeededVariables()
        {
            /*important: the coordinate system starts in the upper left corner*/
            strokeWidth = 5;
            centerX = info.Width / 2;
            centerY = info.Height / 2;
            x1 = strokeWidth / 2;
            y1 = strokeWidth / 2;
            x2 = info.Width - strokeWidth / 2;
            y2 = info.Height - strokeWidth / 2;

            //Debug.WriteLine(string.Format($" centerX: {centerX}, centerY {centerY}, x1: {x1}, x2: {x2}, y1: {y1}, y2: {y2}"));
        }
        static float xPercent(float p)
        {
            float percent = (info.Width - strokeWidth / 2) * p;
            return percent;
        }
        static float yPercent(float p)
        {
            float percent = (info.Height - strokeWidth / 2) * p;
            return percent;
        }
    }
}
