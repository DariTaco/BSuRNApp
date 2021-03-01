using System;
using SkiaSharp.Views.Forms;
using SkiaSharp;

namespace WertheApp.CN
{
    public class AckGenerationDraw
    {
        //VARIABLES
        private static SKCanvasView skiaview;
        private static float xe, ye;
        private static int xStart, xEnd, yStart, yEnd;
        private static float yWidthStep;
        private static int numberOfLines;
        private static int currentStep;
        private static int maxStep;
        private static float textSize, strokeWidth;
        private static SKPaint sk_blackText, sk_PaintThin, sk_PaintVeryThin,
            sk_FastRecovery, sk_CongestionAvoidance, sk_SlowStart,
            sk_PaintArrowPkt, sk_PaintArrowAck, sk_TextArrowPkt, sk_TextArrowAck,
            sk_TextArrowRed, sk_PaintArrowRed, sk_TextSender, sk_TextReceiver,
            sk_blackTextSmall, sk_TextCongestionAvoidance,
            sk_TextFastRecovery, sk_PaintClock,
            sk_TextSlowStart;
        private static String[,] action;
        private static int arrowLength;

        //CONSTRUCTOR
        public AckGenerationDraw()
        {
            // crate the canvas
            skiaview = new SKCanvasView();
            skiaview.PaintSurface += PaintSurface;

            textSize = 5;
            strokeWidth = 0.2f;

            numberOfLines = 13;
            currentStep = 0;
            maxStep = 21;

            FillAckGeneration();

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
            int state = Int32.Parse(action[currentStep, 5]);
            DrawBackground(canvas, state);

            //draw fast recovery reno
            for (int i = 0; i <= currentStep; i++)
            {
                int round = Int32.Parse(action[i, 0]);
                String txt = action[i, 1] + action[i, 2];
                String kind = action[i, 1];
                String dupAck = action[i, 3];
                String data = action[i, 4];
                String part = action[i, 6];

                switch (kind)
                {
                    case "Ack ":
                        DrawAckArrow(canvas, round, sk_PaintArrowAck, part, dupAck);
                        DrawTextNextToAckArrow(canvas, round, txt, sk_TextArrowAck);
                        break;
                    case "Pkt ":
                        DrawPktArrow(canvas, round, sk_PaintArrowPkt, part, data);
                        DrawTextNextToPktArrow(canvas, round, txt, sk_TextArrowPkt);
                        break;
                    case "!Pkt ":
                        DrawPktArrow(canvas, round, sk_PaintArrowRed, part, data);
                        DrawTextNextToPktArrow(canvas, round, txt.Substring(1), sk_TextArrowRed);
                        break;
                    case "Re-Pkt ":
                        DrawPktArrow(canvas, round, sk_PaintArrowPkt, part, data);
                        DrawTextNextToPktArrow(canvas, round, txt, sk_TextArrowPkt);
                        break;
                }
                //show cwnd and dupAck count in very first round
                if (i == 0)
                {
                    DrawTextDupAck(canvas, round, dupAck, sk_blackTextSmall);
                    DrawTextData(canvas, round, data, sk_blackTextSmall);
                }

                //change timeout
                if(currentStep < 14)
                {
                    DrawTimeout(canvas, 1, 7);
                }else if(currentStep >= 14 && currentStep < 21)
                {
                    DrawTimeout(canvas, 6, 12);
                }else if(currentStep >= 21)
                {
                    //DrawTimeout(canvas, 11, 17);
                }
               

            }


            //execute all drawing actions
            canvas.Flush();
        }

        /**********************************************************************
        *********************************************************************/
        static void FillAckGeneration()
        {
            String pkt = "Pkt ";
            String ack = "Ack ";
            // 0,     1,    2,   3,             4,    5,      7
            // round, kind, nr., dup Ack count, data, state  part
            action = new String[,]
            {
            {"1",   pkt, "92", "0", "8", "0", "start"},
            {"2",   pkt, "100", "0", "20", "0", "start"},
            {"3",   pkt, "120", "0", "15", "0", "start"},
            {"4",   pkt, "135", "0", "6", "0", "start"},
            {"5",   pkt, "141", "0", "16", "0", "start"},
            {"1",   pkt, "92", "0", "8", "0", "arrive"},
            {"2",   "!Pkt ", "100", "20", "5", "0", "arrive"},
            {"3",   pkt, "120", "0", "15", "0", "arrive"},
            {"4",   pkt, "135", "0", "6", "0", "arrive"},
            {"5",   pkt, "141", "0", "16", "0", "arrive"},

            {"6",   ack, "100", "0", "-", "0", "start"},
            {"7",   ack, "100", "0", "-", "0", "start"},
            {"8",   ack, "100", "0", "-", "0", "start"},
            {"9",   ack, "100", "0", "-", "0", "start"},
            {"6",   ack, "100", "0", "-", "0", "arrive"},
            {"7",   ack, "100", "1", "-", "0", "arrive"},
            {"8",   ack, "100", "2", "-", "0", "arrive"},
            {"9",   ack, "100", "3", "-", "2", "arrive"},

            {"10",   pkt, "100", "4", "20", "2", "start"},
            {"10",   pkt, "100", "4", "20", "2", "arrive"},

            {"11",   ack, "157", "0", "-", "2", "start"},
            {"11",   ack, "157", "0", "-", "1", "arrive"}
            };
        }
        /**********************************************************************
        *********************************************************************/
        static void DrawAnalogClock(SKCanvas canvas)
        {
            canvas.Translate(38 * xe, 2.5f * ye);
            //canvas.Scale(0.5f);
            var clockPath1 = SKPath.ParseSvgPathData("M46.907,20.12c-0.163-0.347-0.511-0.569-0.896-0.569h-2.927C41.223,9.452,32.355,1.775,21.726,1.775C9.747,1.775,0,11.522,0,23.501C0,35.48,9.746,45.226,21.726,45.226c7.731,0,14.941-4.161,18.816-10.857 c0.546-0.945,0.224-2.152-0.722-2.699c-0.944-0.547-2.152-0.225-2.697,0.72c-3.172,5.481-9.072,8.887-15.397,8.887c-9.801,0-17.776-7.974-17.776-17.774c0-9.802,7.975-17.776,17.776-17.776c8.442,0,15.515,5.921,17.317,13.825h-2.904c-0.385,0-0.732,0.222-0.896,0.569c-0.163,0.347-0.11,0.756,0.136,1.051l4.938,5.925c0.188,0.225,0.465,0.355,0.759,0.355c0.293,0,0.571-0.131,0.758-0.355l4.938-5.925C47.018,20.876,47.07,20.467,46.907,20.12z");
            var clockPath2 = SKPath.ParseSvgPathData("M21.726,6.713c-1.091,0-1.975,0.884-1.975,1.975v11.984c-0.893,0.626-1.481,1.658-1.481,2.83c0,1.906,1.551,3.457,3.457,3.457c0.522,0,1.014-0.125,1.458-0.334l6.87,3.965c0.312,0.181,0.65,0.266,0.986,0.266c0.682,0,1.346-0.354,1.712-0.988c0.545-0.943,0.222-2.152-0.724-2.697l-6.877-3.971c-0.092-1.044-0.635-1.956-1.449-2.526V8.688C23.701,7.598,22.816,6.713,21.726,6.713z M21.726,24.982c-0.817,0-1.481-0.665-1.481-1.48c0-0.816,0.665-1.481,1.481-1.481s1.481,0.665,1.481,1.481C23.207,24.317,22.542,24.982,21.726,24.982z");
            canvas.DrawPath(clockPath1, sk_PaintThin);
            canvas.DrawPath(clockPath2, sk_PaintThin);
            //canvas.Scale(2f);
            canvas.Translate(-38 * xe, 2.5f * ye);
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

            //draw captions
            canvas.DrawText("dAck", 10 * xe, yStart / 1f * ye, sk_blackText);
            canvas.DrawText("data", 22.5f * xe, yStart / 1f * ye, sk_blackText);
            canvas.DrawText("t/o", 35 * xe, yStart / 1f * ye, sk_blackText);
            //DrawAnalogClock(canvas);

            //draw "sender" and line for sender (left)
            canvas.DrawText("S", xStart * xe, yStart / 1.5f * ye, sk_TextSender);
            canvas.DrawLine(new SKPoint(xStart * xe, yEnd * ye), new SKPoint(xStart * xe, yStart * ye), sk_PaintThin);
            //draw "receiver" and line for receiver (right)
            canvas.DrawText("R", xEnd * xe, yStart / 1.5f * ye, sk_TextReceiver);
            canvas.DrawLine(new SKPoint(xEnd * xe, yEnd * ye), new SKPoint(xEnd * xe, yStart * ye), sk_PaintThin);


            //draw horizontal lines
            float posY = yStart;
            for (int i = 0; i <= numberOfLines; i++)
            {
                canvas.DrawLine(new SKPoint(xStart * xe, posY * ye), new SKPoint(xEnd * xe, posY * ye), sk_PaintVeryThin);
                posY += yWidthStep;
            }

        }

        /**********************************************************************
        *********************************************************************/
        static void DrawTimeout(SKCanvas canvas, int roundBegin, int roundEnd)
        {
            int roundText = roundBegin + (roundEnd - roundBegin) / 2;
            SKPoint timeoutBegin = new SKPoint(35 * xe, (yStart + yWidthStep * roundBegin) * ye);
            SKPoint timeoutEnd = new SKPoint(35 * xe, (yStart + yWidthStep * roundEnd) * ye);
            canvas.DrawLine(timeoutBegin, timeoutEnd, sk_PaintThin);
            canvas.DrawLine(timeoutBegin, new SKPoint(40 * xe, (yStart + yWidthStep * roundBegin) * ye), sk_PaintVeryThin);
            canvas.DrawLine(timeoutEnd, new SKPoint(40 * xe, (yStart + yWidthStep * roundEnd) * ye), sk_PaintVeryThin);

        }

        /**********************************************************************
        *********************************************************************/
        static void DrawTextDupAck(SKCanvas canvas, int round, String txt, SKPaint paint)
        {
            canvas.DrawText(txt, 10 * xe, ((yStart + yWidthStep * round) + (textSize / 2f) / 3) * ye, paint);
        }

        /**********************************************************************
         *********************************************************************/
        static void DrawTextData(SKCanvas canvas, int round, String txt, SKPaint paint)
        {
            canvas.DrawText(txt, 22.5f * xe, ((yStart + yWidthStep * round) + (textSize / 2f) / 3) * ye, paint);
        }

        /**********************************************************************
        *********************************************************************/
        static void DrawTextNextToPktArrow(SKCanvas canvas, int round, String txt, SKPaint paint)
        {
            canvas.DrawText(txt, (xStart + arrowLength * 2) * xe, ((yStart + yWidthStep * round) + (textSize / 2f) / 3) * ye, paint);
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
        static void DrawPktArrow(SKCanvas canvas, int round, SKPaint paint, String part, String data)
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
                    DrawTextData(canvas, round, data, sk_blackTextSmall);
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
                arrowEnd = new SKPoint((xEnd - arrowLength / 2) * xe, (yStart + yWidthStep * round) * ye);
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
        static void DrawAckArrow(SKCanvas canvas, int round, SKPaint paint, String part, String dupAck)
        {
            
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
            yWidthStep = yLength / numberOfLines;
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

            sk_PaintClock = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth / 5f * xe,
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
