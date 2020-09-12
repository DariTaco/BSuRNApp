using System;
using System.Collections.Generic;
using System.Diagnostics;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace WertheApp.BS
{
    public class DeadlockViewCell: ViewCell
    {
        //VARIABLES
        private SKCanvas canvas;
        private SKCanvasView skiaview;
        private float xe, ye;
        private SKPaint sk_Paint1, sk_blackText, sk_AText, sk_BText, sk_CText, sk_EText,
            sk_BackgroundBlue, sk_BackgroundRed, sk_BackgroundYellow, sk_BackgroundGreen;

        private int cellNumber;

        private String vectorE, vectorB, vectorC, vectorA;
        private Dictionary<int, String> vectorBProcesses, vectorCProcesses;
        private int totalProcesses;

        // Click Sensitive Areas
        private SKRect rect_CP1, rect_CP2, rect_CP3, rect_CP4, rect_CP5;


        public DeadlockViewCell()
        {
            // crate the canvas
            this.skiaview = new SKCanvasView();
            this.skiaview.PaintSurface += PaintSurface;

            // assign the canvas to the cell
            this.View = this.skiaview;

            this.cellNumber = Deadlock.GetCellNumber();
            this.vectorE = Deadlock.GetVectorE();
            this.vectorB = Deadlock.GetVectorB();
            this.vectorC = Deadlock.GetVectorC();
            this.vectorA = Deadlock.GetVectorA();
            this.vectorCProcesses = Deadlock.GetVectorCProcesses();
            vectorBProcesses = new Dictionary<int, string> { };
            this.vectorBProcesses = Deadlock.GetVectorBProcesses();
            this.totalProcesses = Deadlock.GetTotalProcesses();

        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        // do the drawing
        void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            //canvas object
            SKSurface surface = e.Surface;
            this.canvas = surface.Canvas;

            //Important! Otherwise the drawing will look messed up in iOS
            if (this.canvas != null) { this.canvas.Clear(); }

            //calculate some stuff and make the paint
            CalculateNeededNumbers();
            MakeSKPaint(); //depends on xe and ye and therfore has to be called after they were initialized

            /*********************HERE GOES THE DRAWING************************/
         

            if(this.cellNumber == -1)
            {
                //DrawBackground(this.canvas, sk_Background);
                DrawFirstCell(this.canvas);
            }
            else
            {
                //DrawBackground(this.canvas, sk_BackgroundRed);
                DrawCell(this.canvas);
            }

            //execute all drawing actions
            this.canvas.Flush();
        }


        /* TOUCH SENISITIVIY
        **********************************************************************
       *********************************************************************/
        private async void OnTouch(object sender, SKTouchEventArgs e)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:

                    if (rect_CP1.Contains(e.Location))
                    {
                        Deadlock.CP1_Clicked();
                        this.skiaview.EnableTouchEvents = false; 
                    }
                    else if (rect_CP2.Contains(e.Location))
                    {
                        Deadlock.CP2_Clicked();
                        this.skiaview.EnableTouchEvents = false;
                    }
                    else if (rect_CP3.Contains(e.Location))
                    {
                        Deadlock.CP3_Clicked();
                        this.skiaview.EnableTouchEvents = false;
                    }
                    else if (rect_CP4.Contains(e.Location))
                    {
                        Deadlock.CP4_Clicked();
                        this.skiaview.EnableTouchEvents = false;
                    }
                    else if (rect_CP5.Contains(e.Location))
                    {
                        Deadlock.CP5_Clicked();
                        this.skiaview.EnableTouchEvents = false;
                    }
                    break;
            }

            e.Handled = true;
        }
        private void CreateTouchSensitiveAreas()
        {
            int startx = 64;
            int endx = 92;
            int starty = 1;
            int step = 20;

            //TODO: Depending on how many processes create amount of touch sensitive areas
            //make zero for P4 & 5 of only 3 Processes are existent for example
            rect_CP1 = new SKRect(xe * startx, ye * (starty + step * 0), xe * endx, ye * (starty + step * 1));
            //this.canvas.DrawRect(rect_CP1, sk_BackgroundRed);
            rect_CP2 = new SKRect(xe * startx, ye * (starty + step * 1), xe * endx, ye * (starty + step * 2));
            rect_CP3 = new SKRect(xe * startx, ye * (starty + step * 2), xe * endx, ye * (starty + step * 3));
            //this.canvas.DrawRect(rect_CP3, sk_BackgroundRed);
            rect_CP4 = new SKRect(xe * startx, ye * (starty + step * 3), xe * endx, ye * (starty + step * 4));
            rect_CP5 = new SKRect(xe * startx, ye * (starty + step * 4), xe * endx, ye * (starty + step * 5));
            //this.canvas.DrawRect(rect_CP5, sk_BackgroundRed);

        }
        /**********************************************************************
        *********************************************************************/
        public void DrawBackground(SKCanvas canvas, SKPaint color)
        {
            SKRect sk_rBackground = new SKRect(xe * 00, ye * 00, xe * 100, ye * 100); //left , top, right, bottom
            this.canvas.DrawRect(sk_rBackground, color); //left, top, right, bottom, color
        }

        /**********************************************************************
        *********************************************************************/
        public void DrawFirstCell(SKCanvas canvas)
        {

            DrawAllVectors(canvas);
            DrawBusyProcesses(canvas);
            DrawUpcomingProcesses(canvas);


            // grid lines
            //showGridLines(canvas);

            // add touch sensitivity
            this.skiaview.Touch += OnTouch;
            this.skiaview.EnableTouchEvents = true;
            CreateTouchSensitiveAreas();
        }

        public void showGridLines(SKCanvas canvas)
        {
            // grid lines
            var startValue = 0;
            for (int i = 0; i <= 100; i += 5)
            {
                SKPoint sk_p1 = new SKPoint(startValue + xe * i, ye * 00);
                SKPoint sk_p2 = new SKPoint(startValue + xe * i, ye * 100);
                canvas.DrawLine(sk_p1, sk_p2, sk_Paint1);
            }
            for (int i = 0; i <= 100; i += 20)
            {
                SKPoint sk_p1 = new SKPoint(startValue + xe * 0, ye * i);
                SKPoint sk_p2 = new SKPoint(startValue + xe * 100, ye * i);
                canvas.DrawLine(sk_p1, sk_p2, sk_Paint1);
            }
        }

        public void DrawAllVectors(SKCanvas canvas)
        {
            //Fromat text
            String textE = "   E = ( ";
            String textB = " - B = ( ";
            String textC = "C = ( ";
            String textA = "= A = ( ";
            String space = "  ";
            for (int i = 0; i < vectorE.Length; i++)
            {
                if (i == vectorE.Length - 1) { space = " )"; }
                textE = textE + vectorE[i] + space;
                textB = textB + vectorB[i] + space;
                textC = textC + vectorC[i] + space;
                textA = textA + vectorA[i] + space;

            }

            int startx = 5;
            int starty = 15;
            int step = 20;
            int x1Backg = 4;
            int x2Backg = 30;
            int y1Backg = 22;
            int stepBackg = 20;

            //Draw blue background
            SKRect sk_rBackground = new SKRect(xe * x1Backg, ye * y1Backg, xe * x2Backg, ye * (y1Backg + stepBackg)); //left , top, right, bottom
            this.canvas.DrawRect(sk_rBackground, sk_BackgroundBlue); //left, top, right, bottom, color
            //Vector E
            SKPoint textPosition = new SKPoint(xe * startx, ye * (starty + step));
            canvas.DrawText(textE, textPosition, sk_blackText);

            //Draw red background
            SKRect sk_rBackground2 = new SKRect(xe * x1Backg, ye * (y1Backg + stepBackg), xe * x2Backg, ye * (y1Backg + stepBackg * 2)); //left , top, right, bottom
            this.canvas.DrawRect(sk_rBackground2, sk_BackgroundRed); //left, top, right, bottom, color
            //Vector B
            SKPoint textPosition2 = new SKPoint(xe * startx, ye * (starty + step * 2));
            canvas.DrawText(textB, textPosition2, sk_blackText);

            //Vector C
            /*
            SKPoint textPosition3 = new SKPoint(xe * startx, ye * (starty + step * 2));
            canvas.DrawText(textC, textPosition3, sk_CText);
            */

            //Draw green background
            SKRect sk_rBackground4 = new SKRect(xe * x1Backg, ye * (y1Backg + stepBackg * 2), xe * x2Backg, ye * (y1Backg + stepBackg * 3)); //left , top, right, bottom
            this.canvas.DrawRect(sk_rBackground4, sk_BackgroundGreen); //left, top, right, bottom, color
            //Vector A
            SKPoint textPosition4 = new SKPoint(xe * startx, ye * (starty + step * 3));
            canvas.DrawText(textA, textPosition4, sk_blackText);
        }

        public void DrawBusyProcesses(SKCanvas canvas)
        {

            //Draw red background
            SKRect sk_rBackground = new SKRect(34 * xe, 2 * ye, 62 * xe, 98 * ye); //left , top, right, bottom
            this.canvas.DrawRect(sk_rBackground, sk_BackgroundRed); //left, top, right, bottom, color

            //Busy Processes
            int startx = 35;
            int starty = 15;
            int step = 20;
            String textBusy = "";
            for (int i = 0; i < totalProcesses; i++)
            {
                //Fromat text
                String inputText = vectorBProcesses[i + 1];
                String resultText = "( ";
                String space = "  ";
                for (int j = 0; j < inputText.Length; j++)
                {
                    if (j == inputText.Length - 1) { space = " )"; }
                    resultText = resultText + inputText[j] + space;
                }

                //Draw formatted text
                SKPoint sk_p = new SKPoint(xe * startx, ye * (starty + step * i));
                textBusy = "B(P" + (i + 1) + ") = " + resultText;
                canvas.DrawText(textBusy, sk_p, sk_blackText);

            }
        }

        public void DrawUpcomingProcesses(SKCanvas canvas)
        {

            //Draw yellow background
            SKRect sk_rBackground = new SKRect(64 * xe, 2 * ye, 92 * xe, 98 * ye); //left , top, right, bottom
            this.canvas.DrawRect(sk_rBackground, sk_BackgroundYellow); //left, top, right, bottom, color

            //Upcoming Processes
            int startx = 65;
            int starty = 15;
            int step = 20;
            String textUpcoming = "";
            for (int i = 0; i < totalProcesses; i++)
            {
                //Fromat text
                String inputText = vectorCProcesses[i + 1];
                String resultText = "( ";
                String space = "  ";
                for (int j = 0; j < inputText.Length; j++)
                {
                    if (j == inputText.Length - 1) { space = " )"; }
                    resultText = resultText + inputText[j] + space;
                }

                SKPoint sk_p = new SKPoint(xe * startx, ye * (starty + step * i));
                textUpcoming = "C(P" + (i + 1) + ") = " + resultText;
                canvas.DrawText(textUpcoming, sk_p, sk_blackText);

            }
        }

        /**********************************************************************
        *********************************************************************/
        public void DrawCell(SKCanvas canvas)
        {
            int startx = 5;
            int starty = 35;
            int step = 20;
            int x1Backg = 5;
            int x2Backg = 30;
            int y1Backg = 22;
            int stepBackg = 20;

            //Draw yellow background
            SKRect sk_rBackground = new SKRect(xe * x1Backg, ye * y1Backg, xe * x2Backg, ye * (y1Backg + stepBackg)); //left , top, right, bottom
            this.canvas.DrawRect(sk_rBackground, sk_BackgroundYellow); //left, top, right, bottom, color
            // C(x)
            SKPoint textPosition2 = new SKPoint(xe * startx, ye * starty);
            canvas.DrawText("C(Px) = ( x x x x ) ", textPosition2, sk_blackText);

            //Draw red background
            SKRect sk_rBackground2 = new SKRect(xe * x1Backg, ye * (y1Backg + stepBackg), xe * x2Backg, ye * (y1Backg + stepBackg * 2)); //left , top, right, bottom
            this.canvas.DrawRect(sk_rBackground2, sk_BackgroundRed); //left, top, right, bottom, color
            // B(x)
            SKPoint textPosition3 = new SKPoint(xe * startx, ye * (starty + step));
            canvas.DrawText("B(Px) = ( x x x x ) ", textPosition3, sk_blackText);

            //Draw green background
            SKRect sk_rBackground4 = new SKRect(xe * x1Backg, ye * (y1Backg + stepBackg * 2), xe * x2Backg, ye * (y1Backg + stepBackg * 3)); //left , top, right, bottom
            this.canvas.DrawRect(sk_rBackground4, sk_BackgroundGreen); //left, top, right, bottom, color
            // Aneu
            SKPoint textPosition4 = new SKPoint(xe * startx, ye * (starty + step * 2));
            canvas.DrawText("Anew = ( x x x x ) ", textPosition4, sk_blackText);

            DrawBusyProcesses(canvas);
            DrawUpcomingProcesses(canvas);

            // add touch sensitivity
            this.skiaview.Touch += OnTouch;
            this.skiaview.EnableTouchEvents = true;
            CreateTouchSensitiveAreas();
        }

        /**********************************************************************
        *********************************************************************/
        private void MakeSKPaint()
        {
            int vectorTextSize = 12;
            //black neutral text
            sk_blackText = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye * vectorTextSize
            };
            sk_EText = new SKPaint
            {
                Color = SKColors.Blue,
                TextSize = ye * vectorTextSize
            };
            sk_BText = new SKPaint
            {
                Color = SKColors.Red,
                TextSize = ye * vectorTextSize
            };
            sk_CText = new SKPaint
            {
                Color = SKColors.Orange,
                TextSize = ye * vectorTextSize
            };
            sk_AText = new SKPaint
            {
                Color = SKColors.Green,
                TextSize = ye * vectorTextSize
            };


            sk_Paint1 = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(0, 0, 0) //black
            };

            sk_BackgroundBlue = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(67, 110, 238).WithAlpha(30)
            };

            sk_BackgroundRed = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(238, 130, 238).WithAlpha(30)
            };

            sk_BackgroundYellow = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(255, 255, 0).WithAlpha(30)
            };

            sk_BackgroundGreen = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(172, 255, 47).WithAlpha(30)
            };
        }


        /**********************************************************************
        *********************************************************************/
        public SKCanvasView ReturnCanvas()
        {
            return this.skiaview;
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
        void CalculateNeededNumbers()
        {
            /*important: the coordinate system starts in the upper left corner*/
            float lborder = this.canvas.LocalClipBounds.Left;
            float tborder = this.canvas.LocalClipBounds.Top;
            float rborder = this.canvas.LocalClipBounds.Right;
            float bborder = this.canvas.LocalClipBounds.Bottom;

            xe = rborder / 100; //using the variable surfacewidth instead would mess everything up
            ye = bborder / 100;
        }
    }
}

