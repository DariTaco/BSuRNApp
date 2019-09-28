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

        private static SKPaint sk_blackText, sk_PaintThin, sk_PaintVeryThin, sk_FastRecovery,
            sk_PaintArrowPkt, sk_PaintArrowAck, sk_TextArrowPkt, sk_TextArrowAck,
            sk_TextArrowRed, sk_PaintArrowRed;
        private static float textSize, strokeWidth;
        private static float yWidthStep;
        private static int numberOfSteps;
        private static int currentStep;
        private static int maxStep;

        private static String[,] action;

        //CONSTRUCTOR
        public TCPDraw(String s)
        {
            // crate the canvas
            skiaview = new SKCanvasView();
            skiaview.PaintSurface += PaintSurface;

            textSize = 5;
            strokeWidth = 0.2f;

            numberOfSteps = 28;
            currentStep = 0;

            switch (s)
            {
                case "Reno Fast Recovery": fillRenoFastRecovery();
                    maxStep = 24;
                    break;
                case "Ack Generation":
                    maxStep = 30;
                    break;
            }
            
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

            //TODO: background ändern je nach state
            //draw fast recovery reno
            for (int i = 0; i <= currentStep; i++)
            {
                int round = Int32.Parse(action[i, 0]);
                String txt = action[i, 1] + action[i, 2];
                String kind = action[i, 1];

                switch(kind)
                {
                    case "Ack":
                    DrawAckArrow(canvas, round, sk_PaintArrowAck);
                    DrawTextNextToAckArrow(canvas, round, txt, sk_TextArrowAck);
                        break;
                    case "Pkt":
                        DrawPktArrow(canvas, round , sk_PaintArrowPkt);
                        DrawTextNextToPktArrow(canvas, round, txt, sk_TextArrowPkt);
                        break;
                    case "!Pkt":
                        DrawPktArrow(canvas, round, sk_PaintArrowRed);
                        DrawTextNextToPktArrow(canvas, round, txt, sk_TextArrowRed);
                        break;
                    case "Re-Pkt":
                        DrawPktArrow(canvas, round, sk_PaintArrowPkt);
                        DrawTextNextToPktArrow(canvas, round, txt, sk_TextArrowPkt);
                        break;
                }
            }

     





            //execute all drawing actions
            canvas.Flush();

        }
        /**********************************************************************
        *********************************************************************/
        static void fillRenoFastRecovery()
        {
            // round, kind, nr., dup Ack count, cwnd, tresh, state
            action = new String[,]
            {
            {"1","Pkt", "0", "0", "1", "-", "-"},
            {"2", "Ack", "1", "0", "2", "-", "-"},

            {"6", "Pkt", "1", "0", "2", "-", "-"}, {"7", "Pkt", "2", "0", "2", "-", "-"},
            {"7", "Ack", "2", "0", "3", "-", "-"}, {"8", "Ack",  "3", "0", "4", "-", "-"},

            {"11", "Pkt", "3", "0", "4", "-", "-"}, {"12", "Pkt", "4", "0", "4", "-", "-"}, {"13", "!Pkt", "5", "0", "4", "-", "-"}, {"14", "Pkt", "6", "0", "4", "-", "-"},
            {"12", "Ack", "4", "0", "5", "-", "-"}, {"13", "Ack", "5", "0", "6", "-", "-"}, {"15", "Ack",  "5", "1", "6", "-", "-"},

            {"16", "Pkt", "7", "1", "6", "-", "-"}, {"17", "Pkt", "8", "1", "6", "-", "-"}, {"18", "Pkt", "9", "1", "6", "-", "-"}, {"19", "Pkt", "10", "1", "6", "-", "-"},
            {"17", "Ack", "5", "2", "6", "-", "-"}, {"18", "Ack", "5", "3", "6", "-", "-"}, {"19", "Ack", "5", "4", "7", "-", "-"}, {"20", "Ack", "5", "5", "8", "-", "-"},

            {"22", "Re-Pkt", "5", "5", "8", "-", "-"}, {"23", "Pkt", "11", "0", "8", "-", "-"}, {"24", "Pkt", "12", "0", "8", "-", "-"},
            {"23", "Ack", "11", "0", "3", "-", "-"}
            };

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
        static void DrawTextNextToPktArrow(SKCanvas canvas, int round, String txt, SKPaint paint)
        {
            canvas.DrawText(txt, (xStart - xStart/3) * xe, (yStart + yWidthStep * round) * ye, paint);
        }

        /**********************************************************************
        *********************************************************************/
        static void DrawTextNextToAckArrow(SKCanvas canvas, int round, String txt, SKPaint paint)
        {
            canvas.DrawText(txt, (xEnd + xStart/3) * xe, (yStart + yWidthStep * round) * ye, paint);
        }

        /**********************************************************************
        *********************************************************************/
        // draws an arrow from sender to receiver for the given round
        static void DrawPktArrow(SKCanvas canvas, int round, SKPaint paint)
        {
            // arrow line
            SKPoint arrowBegin = new SKPoint(xStart * xe, (yStart + yWidthStep * round) * ye);
            SKPoint arrowEnd = new SKPoint(xEnd * xe, (yStart + yWidthStep * (round + 1)) * ye);
            canvas.DrawLine(arrowBegin, arrowEnd, paint);

            // arrow head
            //calculate stuff
            double a = Math.Abs(arrowBegin.Y - arrowEnd.Y);
            double b = Math.Abs(arrowBegin.X - arrowEnd.X);

            //arrow head: calculate rotation degree
            double cos = a / b;
            double arcCos = Math.Acos(cos);
            double degree = arcCos * (180.0 / Math.PI);
            float rotationDegree = (float)degree - 45f;

            //arrow head: draw 
            SKPath head = new SKPath();
            head.MoveTo(arrowEnd);
            head.RLineTo(-5, -20);
            head.RLineTo(-15, 15);
            head.Close();
            var rotate = SKMatrix.MakeRotationDegrees(-rotationDegree, arrowEnd.X, arrowEnd.Y);
            head.Transform(rotate);
            canvas.DrawPath(head, paint);

        }

        /**********************************************************************
        *********************************************************************/
        // draws an arrow from receiver to sender for the given round
        static void DrawAckArrow(SKCanvas canvas, int round, SKPaint paint)
        {
            // arrow line
            SKPoint arrowBegin = new SKPoint(xEnd * xe, (yStart + yWidthStep * round) * ye);
            SKPoint arrowEnd= new SKPoint(xStart * xe, (yStart + yWidthStep * (round + 4)) * ye);
            canvas.DrawLine(arrowBegin, arrowEnd, paint);

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
            canvas.DrawPath(head, paint);
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

            sk_TextArrowPkt = new SKPaint
            {
                Color = new SKColor(0, 100, 0), //green
                TextSize = ye * textSize / 2f,
                IsAntialias = true,
                IsStroke = false, //TODO: somehow since the newest update this doesnt work anymore for ios
                TextAlign = SKTextAlign.Center,
                IsVerticalText = false
            };

            sk_TextArrowAck = new SKPaint
            {
                Color = new SKColor(0, 0, 100), //blue
                TextSize = ye * textSize / 2f,
                IsAntialias = true,
                IsStroke = false, //TODO: somehow since the newest update this doesnt work anymore for ios
                TextAlign = SKTextAlign.Center,
                IsVerticalText = false
            };

            sk_TextArrowRed = new SKPaint
            {
                Color = new SKColor(100, 0, 0), //red
                TextSize = ye * textSize / 2f,
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

            sk_PaintArrowAck = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(0, 0, 100) //blue
            };

            sk_PaintArrowPkt = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(0, 100, 0) //green
            };

            sk_PaintArrowRed = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth * xe,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                Color = new SKColor(100, 0, 0) //red
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
        public static bool NextStep()
        {
            currentStep++;

            if (currentStep >= maxStep)
            {
                return false;
            }
            return true;
       
        }

        /**********************************************************************
         *********************************************************************/
        public static bool PreviousStep()
        {
            currentStep--;
            if (currentStep <= 0)
            {
                return false;
            }
            return true;
        }


        /**********************************************************************
         *********************************************************************/
         public static int GetCurrentStep()
        {
            return currentStep;
        }

        /**********************************************************************
        *********************************************************************/
        public static String GetCwnd()
        {
            return action[currentStep, 4];
        }

        /**********************************************************************
        *********************************************************************/
        public static String GetDupAckCount()
        {
            return action[currentStep, 3];
        }

        /**********************************************************************
        *********************************************************************/
        public static String GetTresh()
        {
            return action[currentStep, 5];
        }
    }
}
