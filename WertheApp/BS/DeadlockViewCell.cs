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
        SKSurface surface;
        private SKCanvasView skiaview;
        private float xe, ye;
        private SKPaint sk_Paint1, sk_blackText, sk_AText, sk_BText, sk_CText, sk_EText,
            sk_BackgroundBlue, sk_BackgroundRed, sk_BackgroundYellow, sk_BackgroundGreen,
            sk_BackgroundWhite;

        private int cellNumber = 0;
        private static int cellCount = -1;

        private String vectorE, vectorB, vectorC, vectorA;
        private Dictionary<int, String> vectorBProcesses, vectorCProcesses;
        private int totalProcesses;
        private List<int> doneProcesses;
        public bool P1done, P2done, P3done, P4done, P5done;

        // Click Sensitive Areas
        private SKRect rect_CP1, rect_CP2, rect_CP3, rect_CP4, rect_CP5;
        private bool touchable;

        public DeadlockItem item;


        //custom contructor unfortunately not possible...
        public DeadlockViewCell()
        {
            this.skiaview = new SKCanvasView();
            this.skiaview.PaintSurface += PaintSurface;
            this.View = this.skiaview;

            this.cellNumber = Deadlock.GetCellNumber();
            Debug.WriteLine("cell number " + this.cellNumber);
            this.doneProcesses = Deadlock.GetDoneProcesses();
            this.vectorE = Deadlock.GetVectorE();
            this.vectorB = Deadlock.GetVectorB();
            this.vectorC = Deadlock.GetVectorC();
            this.vectorA = Deadlock.GetVectorA();
            this.vectorCProcesses = Deadlock.GetVectorCProcesses();
            this.vectorBProcesses = Deadlock.GetVectorBProcesses();
            this.totalProcesses = Deadlock.GetTotalProcesses();

            //NOTE: Lists and Arrays in C# do not stay the same
            //and are recreated a lot since they're not by reference.
            //somehow bool, int etc stay the same for this viewcellobject
            //once they're assinged
            this.P1done = Deadlock.P1done;
            this.P2done = Deadlock.P2done;
            this.P3done = Deadlock.P3done;
            this.P4done = Deadlock.P4done;
            this.P5done = Deadlock.P5done;

            touchable = true;
        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        // do the drawing
        //NOTE: GETS CALLED SEVERAL TIMES 
        void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            //canvas object
            surface = e.Surface;
            this.canvas = this.surface.Canvas;

            //Important! Otherwise the drawing will look messed up in iOS
            if (this.canvas != null) { this.canvas.Clear(); }

            //calculate some stuff and make the paint
            CalculateNeededNumbers();
            MakeSKPaint(); //depends on xe and ye and therfore has to be called after they were initialized

            /*********************HERE GOES THE DRAWING************************/

            DrawBackground(this.canvas, sk_BackgroundWhite);

            if (this.cellNumber == -1)
            {
                this.DrawFirstCell(this.canvas);
            }
            else
            {
                //DrawBackground(this.canvas, sk_BackgroundRed);
                this.DrawCell(this.canvas);
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
                        Deadlock.CPx_Clicked(ref this.skiaview, ref this.touchable, 1);
                    }
                    else if (rect_CP2.Contains(e.Location))
                    {
                        Deadlock.CPx_Clicked(ref this.skiaview, ref this.touchable, 2);
                    }
                    else if (rect_CP3.Contains(e.Location))
                    {
                        Deadlock.CPx_Clicked(ref this.skiaview, ref this.touchable, 3);
                    }
                    else if (rect_CP4.Contains(e.Location))
                    {
                        Deadlock.CPx_Clicked(ref this.skiaview, ref this.touchable, 4);
                    }
                    else if (rect_CP5.Contains(e.Location))
                    {
                        Deadlock.CPx_Clicked(ref this.skiaview, ref this.touchable, 5);
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
        public void SetTouchSensitive(bool x)
        {
            this.touchable = x;
            this.skiaview.EnableTouchEvents = x;
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

            Debug.WriteLine("draw first cell");
            this.DrawAllVectors(canvas);
            this.DrawBusyProcesses(canvas);
            this.DrawUpcomingProcesses(canvas);


            // grid lines
            //showGridLines(canvas);

            // add/disble touch sensitivity
            if (this.touchable)
            {
                this.skiaview.Touch += OnTouch;
                this.skiaview.EnableTouchEvents = true;
                this.CreateTouchSensitiveAreas();
            }
            else
            {
                this.skiaview.EnableTouchEvents = false;
            }

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
            canvas.DrawRect(sk_rBackground, sk_BackgroundBlue); //left, top, right, bottom, color
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
            canvas.DrawRect(sk_rBackground, sk_BackgroundRed); //left, top, right, bottom, color

            //Busy Processes
            int startx = 35;
            int starty = 15;
            int step = 20;
            String textBusy = "";
            for (int i = 0; i < this.totalProcesses; i++)
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

                Debug.WriteLine("done Processes" + this.cellNumber);
                Debug.WriteLine("P1" + P1done);
                Debug.WriteLine("P2" + P2done);

                switch (i+1)
                {
                    case 1: if (!P1done) { 
                            SKPoint sk_p = new SKPoint(xe * startx, ye * (starty + step * i));
                            textBusy = "B(P" + (i + 1) + ") = " + resultText;
                            this.canvas.DrawText(textBusy, sk_p, sk_blackText);
                        }; break;
                    case 2: if (!P2done)
                        {
                            SKPoint sk_p = new SKPoint(xe * startx, ye * (starty + step * i));
                            textBusy = "B(P" + (i + 1) + ") = " + resultText;
                            this.canvas.DrawText(textBusy, sk_p, sk_blackText);
                        }; break;
                    case 3: if (!P3done)
                        {
                            SKPoint sk_p = new SKPoint(xe * startx, ye * (starty + step * i));
                            textBusy = "B(P" + (i + 1) + ") = " + resultText;
                            this.canvas.DrawText(textBusy, sk_p, sk_blackText);
                        }; break;
                    case 4: if (!P4done)
                        {
                            SKPoint sk_p = new SKPoint(xe * startx, ye * (starty + step * i));
                            textBusy = "B(P" + (i + 1) + ") = " + resultText;
                            this.canvas.DrawText(textBusy, sk_p, sk_blackText);
                        }; break;
                    case 5: if (!P5done)
                        {
                            SKPoint sk_p = new SKPoint(xe * startx, ye * (starty + step * i));
                            textBusy = "B(P" + (i + 1) + ") = " + resultText;
                            this.canvas.DrawText(textBusy, sk_p, sk_blackText);
                        }; break;

                }
                /*
                String print = "";
                foreach (var pro in doneArr)
                {
                    print = print + pro + ",";
                }
                Debug.WriteLine("done processes array of " + this.cellNumber + ": " + print);

                // draw only Processes that aren't done yet and draw done Processes as marked
                
                if (this.doneProcesses.Contains(i))
                {
                }
                else
                {
                    //Draw formatted text
                    SKPoint sk_p = new SKPoint(xe * startx, ye * (starty + step * i));
                    textBusy = "B(P" + (i + 1) + ") = " + resultText;
                    this.canvas.DrawText(textBusy, sk_p, sk_blackText);

                }*/


            }
        }

        public void DrawUpcomingProcesses(SKCanvas canvas)
        {

            //Draw yellow background
            SKRect sk_rBackground = new SKRect(64 * xe, 2 * ye, 92 * xe, 98 * ye); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground, sk_BackgroundYellow); //left, top, right, bottom, color

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
            Debug.WriteLine("Draw normal cell");
            int startx = 5;
            int starty = 35;
            int step = 20;
            int x1Backg = 5;
            int x2Backg = 30;
            int y1Backg = 22;
            int stepBackg = 20;

            //Draw yellow background
            SKRect sk_rBackground = new SKRect(xe * x1Backg, ye * y1Backg, xe * x2Backg, ye * (y1Backg + stepBackg)); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground, sk_BackgroundYellow); //left, top, right, bottom, color
            // C(x)
            SKPoint textPosition2 = new SKPoint(xe * startx, ye * starty);
            canvas.DrawText("C(Px) = ( x x x x ) ", textPosition2, sk_blackText);

            //Draw red background
            SKRect sk_rBackground2 = new SKRect(xe * x1Backg, ye * (y1Backg + stepBackg), xe * x2Backg, ye * (y1Backg + stepBackg * 2)); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground2, sk_BackgroundRed); //left, top, right, bottom, color
            // B(x)
            SKPoint textPosition3 = new SKPoint(xe * startx, ye * (starty + step));
            canvas.DrawText("B(Px) = ( x x x x ) ", textPosition3, sk_blackText);

            //Draw green background
            SKRect sk_rBackground4 = new SKRect(xe * x1Backg, ye * (y1Backg + stepBackg * 2), xe * x2Backg, ye * (y1Backg + stepBackg * 3)); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground4, sk_BackgroundGreen); //left, top, right, bottom, color
            // Aneu
            SKPoint textPosition4 = new SKPoint(xe * startx, ye * (starty + step * 2));
            canvas.DrawText("Anew = ( x x x x ) ", textPosition4, sk_blackText);

            this.DrawBusyProcesses(canvas);
            this.DrawUpcomingProcesses(canvas);

            // add/disble touch sensitivity
            if (this.touchable)
            {
                this.skiaview.Touch += OnTouch;
                this.skiaview.EnableTouchEvents = true;
                this.CreateTouchSensitiveAreas();
            }
            else
            {
                this.skiaview.EnableTouchEvents = false;
            }
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

            sk_BackgroundWhite = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = SKColors.White
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

