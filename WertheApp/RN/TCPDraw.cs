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

        private static String [] senderText, receiverText;
        private static int[] senderAction, receiverAction;
        private static String[] state;
        private static int[] action;
        private static int [] dupAckCount;
        private static int [] cwnd;
        private static int [] tresh;

        //TODO: andere Farben Pfeile
        //TODO: Pfeil der nicht ankommt zb rot einzeichnen
        //CONSTRUCTOR
        public TCPDraw(String s)
        {
            // crate the canvas
            skiaview = new SKCanvasView();
            skiaview.PaintSurface += PaintSurface;

            textSize = 5;
            strokeWidth = 0.2f;

            numberOfSteps = 28;
            currentStep = -1;

            switch (s)
            {
                case "Reno Fast Recovery": fillRenoFastRecovery();
                    break;
                case "Ack Generation":
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

            //draw pkt and ack
            /*DrawPktArrow(canvas, 1);
            DrawAckArrow(canvas, 2);
            DrawPktArrow(canvas, 7);
            DrawAckArrow(canvas, 8);
            DrawPktArrow(canvas, 13);
            DrawAckArrow(canvas, 14);
            DrawPktArrow(canvas, 19);
            DrawAckArrow(canvas, 20);
            DrawPktArrow(canvas, 25);
            DrawAckArrow(canvas, 26);*/


            //TODO: draw arrows 
            for (int i = 0; i < senderText.Length; i++)
            {
                if(senderText[i] != "")
                {
                    if(senderAction[i] == 2)
                    {
                        DrawPktArrow(canvas, i + 1, sk_PaintArrowRed);
                        DrawTextNextToPktArrow(canvas, i + 1, senderText[i], sk_TextArrowRed);
                    }
                    else
                    {
                        DrawPktArrow(canvas, i + 1, sk_PaintArrowPkt);
                        DrawTextNextToPktArrow(canvas, i + 1, senderText[i], sk_TextArrowPkt);
                    }
                }

                if(receiverText[i] != "")
                {
                    DrawAckArrow(canvas, i+1, sk_PaintArrowAck);
                    DrawTextNextToAckArrow(canvas, i + 1, receiverText[i], sk_TextArrowAck);
                }
            }

     





            //execute all drawing actions
            canvas.Flush();

        }
        /**********************************************************************
        *********************************************************************/
        static void fillRenoFastRecovery()
        {
            senderAction = new int[]
            {   1, 0, 0, 0, 0,
                1, 1, 0, 0, 0,
                1, 1, 2, 1, 0,
                1, 1, 1, 1, 0,
                0, 3, 1, 1, 0
            };

            senderText = new string[]
            {   "Pkt 0", "", "", "", "",
                "Pkt 1", "Pkt 2","", "", "",
                "Pkt 3", "Pkt 4", "Pkt 5", "Pkt 6", "",
                "Pkt 7", "Pkt 8", "Pkt 9", "Pkt 10", "",
                "", "Re-Pkt 5", "Pkt 11", "Pkt 12", ""};

            receiverAction = new int[]
            {   0, 1, 0, 0, 0,
                0, 1, 1, 0, 0,
                0, 1, 1, 0, 1,
                1, 1, 1, 1, 0,
                1, 0, 0, 0, 0

            };

            receiverText = new string[]
            {   "", "Ack 1", "", "", "",
                "", "Ack 2", "Ack 3", "", "",
                "", "Ack 4", "Ack 5", "", "Ack 5", "",
                "Ack 5","Ack 5","Ack 5", "Ack 5","","",
                "Ack 11","","","","","","",""

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
        public static void nextStep()
        {
            currentStep++;
       
        }
    }
}
