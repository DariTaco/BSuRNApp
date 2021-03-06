﻿using System;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Diagnostics; //Debug.WriteLine("");

namespace WertheApp.CN
{
    public class RenoFastRecoveryDraw
    {
        //VARIABLES
        private static SKCanvasView skiaview;
        private static float xe, ye;
        private static int xStart, xEnd, yStart, yEnd;

        private static SKPaint sk_blackText, sk_PaintThin, sk_PaintVeryThin,
            sk_FastRecovery, sk_CongestionAvoidance, sk_SlowStart,
            sk_PaintArrowPkt, sk_PaintArrowAck, sk_TextArrowPkt, sk_TextArrowAck,
            sk_TextArrowRed, sk_PaintArrowRed, sk_TextSender, sk_TextReceiver,
           sk_blackTextSmall,
            sk_TextCongestionAvoidance, sk_TextFastRecovery, sk_TextSlowStart;
        private static float textSize, strokeWidth;
        private static float yWidthStep;
        private static int numberOfSteps;
        private static int currentStep;
        private static int maxStep;
        private static int arrowLength;

        private static String[,] action;

        //CONSTRUCTOR
        public RenoFastRecoveryDraw()
        {
            // crate the canvas
            skiaview = new SKCanvasView();
            skiaview.PaintSurface += PaintSurface;

            textSize = 5;
            strokeWidth = 0.2f;

            numberOfSteps = 37;
            currentStep = 0;
            FillRenoFastRecovery();
            maxStep = 71;

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
            int state = Int32.Parse(action[currentStep, 6]);
            DrawBackground(canvas, state);

            //draw fast recovery reno
            for (int i = 0; i <= currentStep; i++)
            {
                int round = Int32.Parse(action[i, 0]);
                String txt = action[i, 1] + action[i, 2];
                String kind = action[i, 1];
                String dupAck = action[i, 3];
                String cwnd = action[i, 4];
                String tresh = action[i, 5];
                String part = action[i, 7];

                switch (kind)
                {
                    case "Ack ":
                        DrawAckArrow(canvas, round, sk_PaintArrowAck, part, dupAck, cwnd, tresh);
                        DrawTextNextToAckArrow(canvas, round, txt, sk_TextArrowAck);
                        break;
                    case "Pkt ":
                        DrawPktArrow(canvas, round, sk_PaintArrowPkt, part);
                        DrawTextNextToPktArrow(canvas, round, txt, sk_TextArrowPkt);
                        break;
                    case "!Pkt ":
                        DrawPktArrow(canvas, round, sk_PaintArrowRed, part);
                        DrawTextNextToPktArrow(canvas, round, txt.Substring(1), sk_TextArrowRed);
                        break;
                    case "Re-Pkt ":
                        DrawPktArrow(canvas, round, sk_PaintArrowPkt, part);
                        DrawTextNextToPktArrow(canvas, round, txt, sk_TextArrowPkt);
                        break;
                }
                //show cwnd and dupAck count in very first round
                if (i == 0)
                {
                    DrawTextTresh(canvas, round, tresh, sk_blackTextSmall);
                    DrawTextDupAck(canvas, round, dupAck, sk_blackTextSmall);
                    DrawTextCwnd(canvas, round, cwnd, sk_blackTextSmall);
                }

            }
            

            //execute all drawing actions
            canvas.Flush();

        }
        /**********************************************************************
        *********************************************************************/
        static void FillRenoFastRecovery()
        {
            //TODO: correct values
            //state: 0 = slow start, 1 = congestion avoidance, 2 = fast recovery

            String pkt = "Pkt ";
            String ack = "Ack ";

            // 0,     1,    2,   3,             4,    5,     6      7
            // round, kind, nr., dup Ack count, cwnd, tresh, state  part
            action = new String[,]
            {
            {"1",   pkt, "0", "0", "1", "64k", "0", "start"},
            {"1",   pkt, "0", "0", "1", "64k", "0", "arrive"},
            {"2",   ack, "1", "0", "2", "64k", "0", "start"},
            {"2",   ack, "1", "0", "2", "64k", "0", "arrive"},

            {"3", pkt, "1", "0", "2", "64k", "0", "start"},   {"4", pkt,  "2", "0", "2", "64k", "0", "start"},
            {"3", pkt, "1", "0", "2", "64k", "0", "arrive"},  {"4", pkt,  "2", "0", "2", "64k", "0", "arrive"},
            {"5", ack, "2", "0", "3", "64k", "0", "start"},   {"6", ack,  "3", "0", "4", "64k", "0", "start"},
            {"5", ack, "2", "0", "3", "64k", "0", "arrive"},  {"6", ack,  "3", "0", "4", "64k", "0", "arrive"},

            {"7", pkt, "3", "0", "4", "64k", "0", "start"},   {"8",   pkt, "4", "0", "4", "64k", "0", "start"},
            {"9", pkt, "5", "0", "4", "64k", "0", "start"},   {"10",  pkt, "6", "0", "4", "64k", "0", "start"},
            {"7", pkt, "3", "0", "4", "64k", "0", "arrive"},  {"8",   pkt, "4", "0", "4", "64k", "0", "arrive"},
            {"9", "!Pkt ", "5", "0", "4", "64k", "0", "arrive"}, {"10",  pkt, "6", "0", "4", "64k", "0", "arrive"},

            {"11", ack, "4", "0", "5", "64k", "0", "start"},  {"12", ack, "5", "0", "6", "64k", "0", "start"},
            {"13", ack,  "5", "0", "6", "64k", "0", "start"},
            {"11", ack, "4", "0", "5", "64k", "0", "arrive"}, {"12", ack, "5", "0", "6", "64k", "0", "arrive"},
            {"13", ack,  "5", "1", "6", "64k", "0", "arrive"},

            {"14", pkt, "7", "1", "6", "64k", "0", "start"},  {"15", pkt, "8",  "1", "6", "64k", "0", "start"},
            {"16", pkt, "9", "1", "6", "64k", "0", "start"},  {"17", pkt, "10", "1", "6", "64k", "0", "start"},
            {"18", pkt, "11", "1", "6", "64k", "0", "start"}, {"19", pkt, "12", "1", "6", "64k", "0", "start"},
            {"14", pkt, "7", "1", "6", "64k", "0", "arrive"}, {"15", pkt, "8",  "1", "6", "64k", "0", "arrive"},
            {"16", pkt, "9", "1", "6", "64k", "0", "arrive"}, {"17", pkt, "10", "1", "6", "64k", "0", "arrive"},
            {"18", pkt, "11", "1", "6", "64k", "0", "arrive"},{"19", pkt, "12", "1", "6", "64k", "0", "arrive"},

            {"20", ack, "5", "1", "6", "64k", "0", "start"},  {"21", ack, "5", "1", "6", "64k", "0", "start"},
            {"22", ack, "5", "1", "6", "64k", "0", "start"},  {"23", ack, "5", "1", "6", "64k", "0", "start"},
            {"24", ack, "5", "1", "6", "64k", "0", "start"},  {"25", ack, "5", "1", "6", "64k", "0", "start"},
            {"20", ack, "5", "2", "6", "64k", "0", "arrive"}, {"21", ack, "5", "3", "6", "3", "2", "arrive"},
            {"22", ack, "5", "4", "7", "3", "2", "arrive"}, {"23", ack, "5", "5", "8", "3", "2", "arrive"},
            {"24", ack, "5", "6", "9", "3", "2", "arrive"}, {"25", ack, "5", "7", "10", "3", "2", "arrive"},

            {"26", "Re-Pkt ", "5", "7", "8", "3", "2", "start"},   {"27", pkt, "13", "7", "8", "3", "2", "start"},
            {"28", pkt, "14", "7", "8", "3", "2", "start"},       {"29", pkt, "15", "7", "8", "3", "2", "start"},
            {"30", pkt, "16", "7", "8", "3", "2", "start"},       {"31", pkt, "17", "7", "8", "3", "2", "start"},
            {"32", pkt, "18", "7", "8", "3", "2", "start"},       {"33", pkt, "19", "7", "8", "3", "2", "start"},
            {"34", pkt, "20", "7", "8", "3", "2", "start"},       {"35", pkt, "21", "7", "8", "3", "2", "start"},
            {"26", "Re-Pkt ", "5", "0", "8", "3", "2", "arrive"},  {"27", pkt, "13", "0", "8", "3", "2", "arrive"},
            {"28", pkt, "14", "0", "8", "3", "2", "arrive"},      {"29", pkt, "15", "0", "8", "3", "2", "arrive"},
            {"30", pkt, "16", "0", "8", "3", "2", "arrive"},      {"31", pkt, "17", "0", "8", "3", "2", "arrive"},
            {"32", pkt, "18", "0", "8", "3", "2", "arrive"},      {"33", pkt, "19", "0", "8", "3", "2", "arrive"},
            {"34", pkt, "20", "0", "8", "3", "2", "arrive"},      {"35", pkt, "21", "0", "8", "3", "2", "arrive"},

            {"36", ack, "13", "0", "3", "3", "2", "start"},
            {"36", ack, "13", "0", "3", "3", "1", "arrive"}
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

            xStart = 45; //
            xEnd = 90;
            arrowLength = (xEnd - xStart) / 4;
            yStart = 5;
            yEnd = 98;

            float yLength = yEnd - yStart;
            yWidthStep = yLength / numberOfSteps;
        }

        /**********************************************************************
        *********************************************************************/
        static void DrawBackground(SKCanvas canvas, int state)
        {
            SKRect sk_rBackground = new SKRect(00 * xe, 0 * ye, 100 * xe, 100 * ye); //left , top, right, bottom
            switch (state)
            {
                case 0:
                    canvas.DrawRect(sk_rBackground, sk_SlowStart); //left, top, right, bottom, color
                    canvas.DrawText("SLOW", 50f * xe + xe, 45f * ye, sk_TextSlowStart);
                    canvas.DrawText("START", 50f * xe + xe, 55f * ye, sk_TextSlowStart);
                    break;
                case 1:
                    canvas.DrawRect(sk_rBackground, sk_CongestionAvoidance); //left, top, right, bottom, color
                    canvas.DrawText("CONGESTION", 50f * xe + xe, 45f * ye, sk_TextCongestionAvoidance);
                    canvas.DrawText("AVOIDANCE", 50f * xe + xe, 55f * ye, sk_TextCongestionAvoidance);
                    break;
                case 2:
                    canvas.DrawRect(sk_rBackground, sk_FastRecovery); //left, top, right, bottom, color
                    canvas.DrawText("FAST", 50f * xe + xe, 45f * ye, sk_TextFastRecovery);
                    canvas.DrawText("RECOVERY", 50f * xe + xe, 55f * ye, sk_TextFastRecovery);
                    break;
                default: break;
            }

            //draw "cwnd"
            canvas.DrawText("cwnd", 10 * xe, yStart / 1f * ye, sk_blackText);
            //draw "Dup Ack"
            canvas.DrawText("dAck", 21.5f * xe, yStart / 1f * ye, sk_blackText);
            //draw tresh
            canvas.DrawText("sstresh", 35 * xe, yStart / 1f * ye, sk_blackText);
            //draw "sender" and line for sender (left)
            canvas.DrawText("S", xStart * xe, yStart / 1.5f * ye, sk_TextSender);
            canvas.DrawLine(new SKPoint(xStart * xe, yEnd * ye), new SKPoint(xStart * xe, yStart * ye), sk_PaintThin);
            //draw "receiver" and line for receiver (right)
            canvas.DrawText("R", xEnd * xe, yStart / 1.5f * ye, sk_TextReceiver);
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
        static void DrawTextDupAck(SKCanvas canvas, int round, String txt, SKPaint paint)
        {
            canvas.DrawText(txt, 21.5f * xe, ((yStart + yWidthStep * round) + (textSize / 2f) / 3) * ye, paint);
        }

        /**********************************************************************
         *********************************************************************/
        static void DrawTextCwnd(SKCanvas canvas, int round, String txt, SKPaint paint)
        {
            canvas.DrawText(txt, 10 * xe, ((yStart + yWidthStep * round) + (textSize / 2f) / 3) * ye, paint);
        }

        /**********************************************************************
        *********************************************************************/
        static void DrawTextTresh(SKCanvas canvas, int round, String txt, SKPaint paint)
        {
            canvas.DrawText(txt, 35 * xe, ((yStart + yWidthStep * round) + (textSize / 2f) / 3) * ye, paint);
        }

        /**********************************************************************
        *********************************************************************/
        static void DrawTextNextToPktArrow(SKCanvas canvas, int round, String txt, SKPaint paint)
        {
            canvas.DrawText(txt, (xStart + arrowLength * 2) * xe, ((yStart + yWidthStep * round) + (textSize / 2f)/3) * ye, paint);
        }

        /**********************************************************************
        *********************************************************************/
        static void DrawTextNextToAckArrow(SKCanvas canvas, int round, String txt, SKPaint paint)
        {
            canvas.DrawText(txt, (xStart + arrowLength * 2) * xe, ((yStart + yWidthStep * round) + (textSize / 2f) / 3) * ye, paint);
        }

        /**********************************************************************
        *********************************************************************/
        // draws an arrow from sender to receiver for the given round
        static void DrawPktArrow(SKCanvas canvas, int round, SKPaint paint, String part)
        {
            SKPoint arrowBegin, arrowEnd;
            arrowBegin = new SKPoint();
            arrowEnd = new SKPoint();
            switch (part)
            {
                case "start":
                    // arrow start
                    arrowBegin = new SKPoint(xStart * xe, (yStart + yWidthStep * round) * ye);
                    arrowEnd = new SKPoint((xStart + arrowLength) * xe, (yStart + yWidthStep * round) * ye);
                    break;
                case "arrive":
                    // arrow arrive
                    arrowBegin = new SKPoint((xEnd - arrowLength) * xe, (yStart + yWidthStep * round) * ye);
                    arrowEnd = new SKPoint(xEnd * xe, (yStart + yWidthStep * round) * ye);
                    break;
            }
            // drawing a shorter "broken" arrow for failed package transmit
            if (paint == sk_PaintArrowRed && part == "arrive")
            {
                arrowEnd = new SKPoint((xEnd - arrowLength/2) * xe, (yStart + yWidthStep * round) * ye);
                SKPath cross = new SKPath();
                cross.MoveTo((xEnd - arrowLength / 4) * xe, (yStart + yWidthStep * round) * ye);
                cross.RLineTo(-7.5f, -7.5f);
                cross.RLineTo(+7.5f, +7.5f);

                cross.RLineTo(+7.5f, +7.5f);
                cross.RLineTo(-7.5f, -7.5f);

                cross.RLineTo(-7.5f, +7.5f);
                cross.RLineTo(+7.5f, -7.5f);

                cross.RLineTo(+7.5f, -7.5f);
                cross.RLineTo(-7.5f, +7.5f);
                canvas.DrawPath(cross, paint);
            }
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
            var rotate = SKMatrix.CreateRotationDegrees(-rotationDegree, arrowEnd.X, arrowEnd.Y);
            head.Transform(rotate);
            canvas.DrawPath(head, paint);

        }

        /**********************************************************************
        *********************************************************************/
        // draws an arrow from receiver to sender for the given round
        static void DrawAckArrow(SKCanvas canvas, int round, SKPaint paint, String part, String dupAck, String cwnd, String tresh)
        {
            Debug.WriteLine("ARROWLENGTH: " + arrowLength);
            SKPoint arrowBegin, arrowEnd;
            arrowBegin = new SKPoint();
            arrowEnd = new SKPoint();

            switch (part)
            {
                case "start":
                    // arrow start
                    arrowBegin = new SKPoint(xEnd * xe, (yStart + yWidthStep * round) * ye);
                    arrowEnd = new SKPoint((xEnd - arrowLength) * xe, (yStart + yWidthStep * round) * ye);
                    break;
                case "arrive":
                    // arrow arrive
                    arrowBegin = new SKPoint((xStart + arrowLength) * xe, (yStart + yWidthStep * round) * ye);
                    arrowEnd = new SKPoint(xStart * xe, (yStart + yWidthStep * round) * ye);
                    DrawTextDupAck(canvas, round, dupAck, sk_blackTextSmall);
                    DrawTextCwnd(canvas, round, cwnd, sk_blackTextSmall);
                    DrawTextTresh(canvas, round, tresh, sk_blackTextSmall);
                    break;
            }
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
            head.RLineTo(5, -20);
            head.RLineTo(15, 15);
            head.Close();
            var rotate = SKMatrix.CreateRotationDegrees(rotationDegree, arrowEnd.X, arrowEnd.Y);
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
                TextSize = ye * textSize / 1.8f,
                IsAntialias = true,
                IsStroke = false, 
                TextAlign = SKTextAlign.Center,
            };

            sk_blackTextSmall = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye * textSize / 2f,
                IsAntialias = true,
                IsStroke = false,
                TextAlign = SKTextAlign.Center,
            };

            sk_TextArrowPkt = new SKPaint
            {
                Color = new SKColor(0, 100, 0), //green
                TextSize = ye * textSize / 2f,
                IsAntialias = true,
                IsStroke = false,
                TextAlign = SKTextAlign.Center,
            };

            sk_TextArrowAck = new SKPaint
            {
                Color = new SKColor(0, 0, 100), //blue
                TextSize = ye * textSize / 2f,
                IsAntialias = true,
                IsStroke = false, 
                TextAlign = SKTextAlign.Center,
            };

            sk_TextArrowRed = new SKPaint
            {
                Color = new SKColor(200, 0, 0), //red
                TextSize = ye * textSize / 2f,
                IsAntialias = true,
                IsStroke = false,
                TextAlign = SKTextAlign.Center,
            };

            sk_TextSender = new SKPaint
            {
                Color = new SKColor(0, 100, 0), //green
                TextSize = ye * textSize / 1.5f,
                IsAntialias = true,
                IsStroke = false, 
                TextAlign = SKTextAlign.Center,
            };

            sk_TextReceiver = new SKPaint
            {
                Color = new SKColor(0, 0, 100), //blue
                TextSize = ye * textSize / 1.5f,
                IsAntialias = true,
                IsStroke = false, 
                TextAlign = SKTextAlign.Center,
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
                Color = new SKColor(200, 0, 0) //red
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

            sk_CongestionAvoidance = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(238, 130, 238).WithAlpha(30)
            };


            sk_SlowStart = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(172, 255, 47).WithAlpha(30)
            };

            sk_TextSlowStart = new SKPaint
            {
                Color = new SKColor(147, 230, 22).WithAlpha(50),
                TextSize = ye * textSize * 1.7f,
                IsAntialias = true,
                IsStroke = false, 
                TextAlign = SKTextAlign.Center,
                FakeBoldText = true
            };

            sk_TextCongestionAvoidance = new SKPaint
            {
                Color = new SKColor(238, 130, 238).WithAlpha(40),
                TextSize = ye * textSize * 1.7f,
                IsAntialias = true,
                IsStroke = false,
                TextAlign = SKTextAlign.Center,
                FakeBoldText = true
            };

            sk_TextFastRecovery = new SKPaint
            {
                Color = new SKColor(67, 110, 238).WithAlpha(30),
                TextSize = ye * textSize * 1.7f,
                IsAntialias = true,
                IsStroke = false,
                TextAlign = SKTextAlign.Center,
                FakeBoldText = true
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


        /**********************************************************************
        *********************************************************************/
        public static void Restart()
        {
            currentStep = 0;
        }


        /**********************************************************************
        *********************************************************************/
        public static void GoToEnd()
        {
            currentStep = maxStep;
        }
    }
}
