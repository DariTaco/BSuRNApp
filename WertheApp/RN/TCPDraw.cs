using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Diagnostics; //Debug.WriteLine("");
using System.Collections.Generic;

namespace WertheApp.RN
{
    public class TCPDraw
    {
        //VARIABLES
        private static SKCanvasView skiaview;
        private static float xe, ye;
        private static int xStart, xEnd, yStart, yEnd;

        private static SKPaint sk_blackText, sk_PaintThin, sk_PaintVeryThin, sk_FastRecovery;
        private static float textSize, strokeWidth;
        private static float yWidthStep;
        private static int numberOfSteps;
        private static int currentStep;

        private static int [,] renoFastRecovery;

        //CONSTRUCTOR
        public TCPDraw()
        {
            // crate the canvas
            skiaview = new SKCanvasView();
            skiaview.PaintSurface += PaintSurface;

            textSize = 5;
            strokeWidth = 0.2f;

            numberOfSteps = 30;
            currentStep = -1;

            /*rows:
             0:action 0->none 1->ack 2->pack 3->pause sender 4->pause receiver
             1:dupAck count
             2:cwnd
             3:state 0->none 1->slow start 2->congestion avoidance 3->fast recovery
             4:treshold
             5:number of ack/pack/etc
             */
            renoFastRecovery = new int[6, numberOfSteps];
        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        // do the drawing
        static void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            //canvas object
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            //Important! Otherwise the drawing will look messed up in iOS
            if (canvas != null)
            {
                canvas.Clear(); 
            }

            //calculate some stuff and make the paint
            CalculateNeededNumbers(canvas);
            MakeSKPaint(); //depends on xe and ye and therfore has to be called after they were initialized

            /*********************HERE GOES THE DRAWING************************/

            DrawBackground(canvas);

            //draw pkt
            DrawPktArrow(canvas, 2);
            DrawAckArrow(canvas, 3);

            //TODO: draw arrows 
            for(int i = 0; i <= currentStep; i++)
            {
                
            }
            //execute all drawing actions
            canvas.Flush();

        }
        /**********************************************************************
        *********************************************************************/
        static String getActionString(int action, int number)
        {
            String s = "";
            switch (action)
            {
                case 0: s = " ";
                    break;
                case 1: s = "Ack " + number.ToString();
                    break;
                case 2: s = "Pkt " + number.ToString();
                    break;
                case 3: s = number.ToString() + "ms";
                    break;
            }
            return s;
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

            xStart = 30; //
            xEnd = 70;
            yStart = 5;
            yEnd = 98;
            float yLength = yEnd - yStart;
            yWidthStep = yLength / numberOfSteps;
        }

        /**********************************************************************
        *********************************************************************/
        static void DrawBackground(SKCanvas canvas)
        {
            SKRect sk_rBackground = new SKRect(00 * xe, 0 * ye, 100 * xe, 100 * ye); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground, sk_FastRecovery); //left, top, right, bottom, color


            //draw "sender" and line for sender (left)
            canvas.DrawText("sender", xStart * xe, yStart / 1.5f * ye, sk_blackText);
            canvas.DrawLine(new SKPoint(xStart * xe, yEnd * ye), new SKPoint(xStart * xe, yStart * ye), sk_PaintThin);
            //draw "receiver" and line for receiver (right)
            canvas.DrawText("receiver", xEnd * xe, yStart / 1.5f * ye, sk_blackText);
            canvas.DrawLine(new SKPoint(xEnd * xe, yEnd * ye), new SKPoint(xEnd * xe, yStart * ye), sk_PaintThin);


            //draw horizontal lines
            float posY = yStart;
            for (int i = 0; i <= numberOfSteps; i++)
            {
                canvas.DrawLine(new SKPoint(xStart * xe, posY * ye), new SKPoint(xEnd * xe, posY * ye), sk_PaintVeryThin);
                posY += yWidthStep;
            }

        }

        /**********************************************************************
        *********************************************************************/
        // draws an arrow from sender to receiver for the given round
        static void DrawPktArrow(SKCanvas canvas, int round)
        {
            // arrow line
            SKPoint arrowBegin = new SKPoint(xStart * xe, (yStart + yWidthStep * round) * ye);
            SKPoint arrowEnd = new SKPoint(xEnd * xe, (yStart + yWidthStep * (round + 1)) * ye);
            canvas.DrawLine(arrowBegin, arrowEnd, sk_PaintThin);

            // arrow head
            //calculate stuff
            double a = Math.Abs(arrowBegin.Y - arrowEnd.Y);
            double b = Math.Abs(arrowBegin.X - arrowEnd.X);

            //arrow head: calculate rotation degree
            double cos = a / b;
            double arcCos = Math.Acos(cos);
            double degree = arcCos * (180.0 / Math.PI);
            float rotationDegree = (float)degree - 45f;

            SKPath head = new SKPath();
            head.MoveTo(arrowEnd);
            head.RLineTo(-5, -20);
            head.RLineTo(-15, 15);
            head.Close();
            var rotate = SKMatrix.MakeRotationDegrees(-rotationDegree, arrowEnd.X, arrowEnd.Y);
            head.Transform(rotate);
            canvas.DrawPath(head, sk_PaintThin);

        }

        /**********************************************************************
        *********************************************************************/
        // draws an arrow from receiver to sender for the given round
        static void DrawAckArrow(SKCanvas canvas, int round)
        {
            // arrow line
            SKPoint arrowBegin = new SKPoint(xEnd * xe, (yStart + yWidthStep * round) * ye);
            SKPoint arrowEnd= new SKPoint(xStart * xe, (yStart + yWidthStep * (round + 1)) * ye);
            canvas.DrawLine(arrowBegin, arrowEnd, sk_PaintThin);

            // arrow head
            //calculate stuff
            double a = Math.Abs(arrowBegin.Y - arrowEnd.Y);
            double b = Math.Abs(arrowBegin.X - arrowEnd.X);

            //arrow head: calculate rotation degree
            double cos = a/b;
            double arcCos = Math.Acos(cos);
            double degree = arcCos * (180.0 / Math.PI);
            float rotationDegree = (float) degree-45f;

            //arrow head: draw 
            SKPath head = new SKPath();
            head.MoveTo(arrowEnd);
            head.RLineTo(5, -20);
            head.RLineTo(15, 15);
            head.Close();
            var rotate = SKMatrix.MakeRotationDegrees(rotationDegree, arrowEnd.X, arrowEnd.Y);
            head.Transform(rotate);
            canvas.DrawPath(head, sk_PaintThin);
        }

        /**********************************************************************
        *********************************************************************/
        public static SKCanvasView ReturnCanvas()
        {
            return skiaview;
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
        static private void MakeSKPaint()
        {
            //For Text
            sk_blackText = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye * textSize / 1.4f,
                IsAntialias = true,
                IsStroke = false, //TODO: somehow since the newest update this doesnt work anymore for ios
                TextAlign = SKTextAlign.Center,
                IsVerticalText = false
            };

            sk_PaintVeryThin = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth / 4f * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(0, 0, 0) //black
            };

            sk_PaintThin = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(0, 0, 0) //black
            };

            sk_FastRecovery = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(67, 110, 238).WithAlpha(30)
            };


        }
        /**********************************************************************
        *********************************************************************/
        public static void nextStep()
        {
            currentStep++;
       
        }
    }
}
