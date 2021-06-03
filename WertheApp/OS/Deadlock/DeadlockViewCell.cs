using System;
using System.Collections.Generic;
using System.Diagnostics;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace WertheApp.OS
{
    public class DeadlockViewCell: ViewCell
    {
        //VARIABLES
        private SKCanvas canvas;
        SKSurface surface;
        public SKCanvasView skiaview;
        private SKPaint sk_Paint1, sk_blackText, sk_blackTextSmaller,
            sk_BackgroundBlue, sk_BackgroundRed, sk_BackgroundYellow, sk_BackgroundGreen,
            sk_CheckMarkContour, sk_BackgroundWhite;

        private int cellNumber = 0;

        private String vectorE, vectorB, vectorC, vectorA;
        private Dictionary<int, String> vectorBProcesses, vectorCProcesses;
        private int totalProcesses;
        public bool P1done, P2done, P3done, P4done, P5done;

        // Click Sensitive Areas
        private SKRect rect_CP1, rect_CP2, rect_CP3, rect_CP4, rect_CP5;
        public bool touchable;

        // canvas
        private static SKImageInfo info; // canvas info
        private static float centerX, centerY, x1, x2, y1, y2; // canvas coordinates
        private static float step, backgWindowLength, backgWindowHeight, textSize, spaceBetweenBackgWindows, xStartBackgWindow1, xStartBackgWindow2, xStartBackgWindow3, yStartBackgWindow; //needed for layout
        // painting tools
        private static float strokeWidth; // stroke Width for paint colors


        public DeadlockItem item;
        private String history;

        //custom contructor unfortunately not possible...
        public DeadlockViewCell()
        {
            this.skiaview = new SKCanvasView();
            this.skiaview.BackgroundColor = App._viewBackground;
            this.skiaview.PaintSurface += PaintSurface;
            this.View = this.skiaview;

            this.cellNumber = Deadlock.GetCellNumber();
            this.vectorE = Deadlock.GetVectorE();
            this.vectorB = Deadlock.GetVectorB();
            this.vectorC = Deadlock.GetVectorC();
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
         
            if(cellNumber != -1)
            {
                this.history = Deadlock.GetHistory(cellNumber, 0);
                this.vectorA = Deadlock.GetHistory(cellNumber, 1);
                //.WriteLine("history of cell " + cellNumber + ": " + history);
                //Deadlock.deadLockViewCellCansvasList.Add(this.skiaview);
                //Deadlock.deadLockViewCellTouchableList.Add(this.touchable);

            }
            else
            {
                this.vectorA = Deadlock.GetVectorA();

            }
        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        // do the drawing
        void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            //canvas object
            info = e.Info; //
            surface = e.Surface;
            this.canvas = this.surface.Canvas;

            //Important! Otherwise the drawing will look messed up in iOS
            if (this.canvas != null) { this.canvas.Clear(); }

            //calculate some stuff and make the paint
            CalculateNeededVariables();
            MakeSKPaint(); //depends on xe and ye and therfore has to be called after they were initialized

            /*********************HERE GOES THE DRAWING************************/

            //DrawBackground(this.canvas, sk_BackgroundWhite);

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

            //Debug.WriteLine("Draw canvas of " + this.Id);

        }


            /* TOUCH SENISITIVIY
            **********************************************************************
           *********************************************************************/
            private void OnTouch(object sender, SKTouchEventArgs e)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    //Debug.WriteLine(this.Id);

                    if (rect_CP1.Contains(e.Location))
                    {
                        Deadlock.CPx_Clicked(ref this.skiaview, ref this.touchable,
                            1, this.vectorA);
                    }
                    else if (rect_CP2.Contains(e.Location))
                    {
                        Deadlock.CPx_Clicked(ref this.skiaview, ref this.touchable,
                            2, this.vectorA);
                    }
                    else if (rect_CP3.Contains(e.Location))
                    {
                        Deadlock.CPx_Clicked(ref this.skiaview, ref this.touchable,
                            3, this.vectorA);
                    }
                    else if (rect_CP4.Contains(e.Location))
                    {
                        Deadlock.CPx_Clicked(ref this.skiaview, ref this.touchable,
                            4, this.vectorA);
                    }
                    else if (rect_CP5.Contains(e.Location))
                    {
                        Deadlock.CPx_Clicked(ref this.skiaview, ref this.touchable,
                            5, this.vectorA);
                    }
                    break;
            }

            e.Handled = true;
        }

        private void CreateTouchSensitiveAreas()
        {       

            //make zero for P4 & 5 of only 3 Processes are existent for example
            rect_CP1 = new SKRect(xStartBackgWindow2, yStartBackgWindow + yPercent(step * 0), xStartBackgWindow2 + backgWindowLength, yStartBackgWindow + yPercent(step * 1));
            //this.canvas.DrawRect(rect_CP1, sk_BackgroundRed);
            rect_CP2 = new SKRect(xStartBackgWindow2, yStartBackgWindow + yPercent(step * 1), xStartBackgWindow2 + backgWindowLength, yStartBackgWindow + yPercent(step * 2));
            //this.canvas.DrawRect(rect_CP2, sk_BackgroundGreen);
            rect_CP3 = new SKRect(xStartBackgWindow2, yStartBackgWindow + yPercent(step * 2), xStartBackgWindow2 + backgWindowLength, yStartBackgWindow + yPercent(step * 3));
            //this.canvas.DrawRect(rect_CP3, sk_BackgroundRed);
            rect_CP4 = new SKRect(xStartBackgWindow2, yStartBackgWindow + yPercent(step * 3), xStartBackgWindow2 + backgWindowLength, yStartBackgWindow + yPercent(step * 4));
            //this.canvas.DrawRect(rect_CP4, sk_BackgroundGreen);
            rect_CP5 = new SKRect(xStartBackgWindow2, yStartBackgWindow + yPercent(step * 4), xStartBackgWindow2 + backgWindowLength, yStartBackgWindow + yPercent(step * 5));
            //this.canvas.DrawRect(rect_CP5, sk_BackgroundRed);

        }

        /**********************************************************************
        *********************************************************************/
        public void DrawBackground(SKCanvas canvas, SKPaint color)
        {
            SKRect sk_rBackground = new SKRect(x1, y1, x2, y2); //left , top, right, bottom
            this.canvas.DrawRect(sk_rBackground, color); //left, top, right, bottom, color
        }

        /**********************************************************************
        *********************************************************************/
        public void DrawFirstCell(SKCanvas canvas)
        {

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

        public void DrawDividingLine()
        {
            SKPoint sk_p1 = new SKPoint(x1, y1);
            SKPoint sk_p2 = new SKPoint(x2, y1);
            canvas.DrawLine(sk_p1, sk_p2, sk_Paint1);
        }
        /**********************************************************************
        *********************************************************************/
        public void DrawAllVectors(SKCanvas canvas)
        {
            //Fromat text
            String textE = "   E = ( ";
            String textB = "   B = ( ";
            String textC = "C = ( ";
            String textA = "   A = ( ";
            String space = "  ";
            for (int i = 0; i < vectorE.Length; i++)
            {
                if (i == vectorE.Length - 1) { space = " )"; }
                textE = textE + vectorE[i] + space;
                textB = textB + vectorB[i] + space;
                textC = textC + vectorC[i] + space;
                textA = textA + vectorA[i] + space;

            }

            float starty = 0.15f;
            float y1Backg = 0.22f;
            float stepBackg = 0.20f;


            //Draw blue background
            SKRect sk_rBackground = new SKRect(
                xStartBackgWindow1,
                yPercent(y1Backg),
                xStartBackgWindow1 + backgWindowLength,
                yPercent(y1Backg) + yPercent(stepBackg)); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground, sk_BackgroundWhite); //left, top, right, bottom, color
            canvas.DrawRect(sk_rBackground, sk_BackgroundBlue); //left, top, right, bottom, color
            //Vector E
            SKPoint textPosition = new SKPoint(xStartBackgWindow1, yPercent(starty) + yPercent(step));
            canvas.DrawText(textE, textPosition, sk_blackText);

            //Draw red background
            SKRect sk_rBackground2 = new SKRect(
                xStartBackgWindow1,
                yPercent(y1Backg) + yPercent(stepBackg),
                xStartBackgWindow1 + backgWindowLength,
                yPercent(y1Backg) + yPercent(stepBackg * 2)); //left , top, right, bottom
            this.canvas.DrawRect(sk_rBackground2, sk_BackgroundWhite); //left, top, right, bottom, color
            this.canvas.DrawRect(sk_rBackground2, sk_BackgroundRed); //left, top, right, bottom, color
            //Vector B
            SKPoint textPosition2 = new SKPoint(xStartBackgWindow1, yPercent(starty) + yPercent(step * 2));
            canvas.DrawText(textB, textPosition2, sk_blackText);

            //Vector C
            /*
            SKPoint textPosition3 = new SKPoint(xe * startx, ye * (starty + step * 2));
            canvas.DrawText(textC, textPosition3, sk_CText);
            */

            //Draw green background
            SKRect sk_rBackground4 = new SKRect(
                xStartBackgWindow1,
                yPercent(y1Backg) + yPercent(stepBackg * 2),
                xStartBackgWindow1 + backgWindowLength,
                yPercent(y1Backg) + yPercent(stepBackg * 3)); //left , top, right, bottom
            this.canvas.DrawRect(sk_rBackground4, sk_BackgroundWhite); //left, top, right, bottom, color
            this.canvas.DrawRect(sk_rBackground4, sk_BackgroundGreen); //left, top, right, bottom, color
            //Vector A
            SKPoint textPosition4 = new SKPoint(xStartBackgWindow1, yPercent(starty) + yPercent(step * 3));
            canvas.DrawText(textA, textPosition4, sk_blackText);
        }

        /**********************************************************************
        *********************************************************************/
        public void DrawBusyProcesses(SKCanvas canvas)
        {

            //Busy Processes
            //TODO

            //Draw red background
            SKRect sk_rBackground = new SKRect(
                xStartBackgWindow3,
                yStartBackgWindow,
                xStartBackgWindow3 + backgWindowLength,
                yStartBackgWindow + backgWindowHeight); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground, sk_BackgroundWhite); //left, top, right, bottom, color
            canvas.DrawRect(sk_rBackground, sk_BackgroundRed); //left, top, right, bottom, color

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

                // draw process
                SKPoint sk_p = new SKPoint(xStartBackgWindow3 + xText(0.02f), yStartBackgWindow + yPercent(step * i) + yPercent(step / 2.0f) + xText(0.015f)); //y= where the backg window starts + height of every process so far + half the size of the height of a process, because letters start on the bottom but the canvas starts on the top + a little bit of offset to not start the text right at the beginning of the background window
                switch (i+1)
                {
                    case 1: if (!P1done)
                        {
                            textBusy = "B(P" + (i + 1) + ") = " + resultText;
                        }
                        else
                        {
                            textBusy = "";
                            //DrawCheckMark(sk_p);
                        };
                        break;
                    case 2: if (!P2done)
                        {
                            textBusy = "B(P" + (i + 1) + ") = " + resultText;
                        }
                        else
                        {
                            textBusy = "";
                            //DrawCheckMark(sk_p);
                        };
                        break;
                    case 3: if (!P3done)
                        {
                            textBusy = "B(P" + (i + 1) + ") = " + resultText;
                        }
                        else
                        {
                            textBusy = "";
                            //DrawCheckMark(sk_p);

                        }; break;
                    case 4: if (!P4done)
                        {
                            textBusy = "B(P" + (i + 1) + ") = " + resultText;
                        }
                        else
                        {
                            textBusy = "";
                            //DrawCheckMark(sk_p);

                        }; break;
                    case 5: if (!P5done)
                        {
                            textBusy = "B(P" + (i + 1) + ") = " + resultText;
                        }
                        else
                        {
                            textBusy = "";
                            //DrawCheckMark(sk_p);

                        }; break;
                }
                this.canvas.DrawText(textBusy, sk_p, sk_blackText);

            }
        }

        /**********************************************************************
        *********************************************************************/
        public void DrawCheckMark(SKPoint sk_p)
        {
            //x on router
            SKPath cross = new SKPath();
            cross.MoveTo(sk_p.X + xText(0.2f), sk_p.Y - xText(0.02f));
            cross.RLineTo(+xText(0.02f), +xText(0.02f));
            cross.RLineTo(+xText(0.04f), -xText(0.04f));
            canvas.DrawPath(cross, sk_CheckMarkContour);
        }

        /**********************************************************************
        *********************************************************************/
        public void DrawUpcomingProcesses(SKCanvas canvas)
        {
            //Draw yellow background
            SKRect sk_rBackground = new SKRect(
                xStartBackgWindow2,
                yStartBackgWindow,
                xStartBackgWindow2 + backgWindowLength,
                yStartBackgWindow + backgWindowHeight); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground, sk_BackgroundWhite); //left, top, right, bottom, color
            canvas.DrawRect(sk_rBackground, sk_BackgroundYellow); //left, top, right, bottom, color

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

                // draw process
                SKPoint sk_p = new SKPoint(xStartBackgWindow2 + xText(0.02f), yStartBackgWindow + yPercent(step * i) + yPercent(step/2.0f) + xText(0.015f)); //y= where the backg window starts + height of every process so far + half the size of the height of a process, because letters start on the bottom but the canvas starts on the top + a little bit of offset to not start the text right at the beginning of the background window
                switch (i + 1)
                {
                    case 1:
                        if (!P1done)
                        {
                            textUpcoming = "C(P" + (i + 1) + ") = " + resultText;
                        }
                        else
                        {
                            textUpcoming = "P1 ";
                            //textUpcoming = "B(P" + (i + 1) + ") ";
                            DrawCheckMark(sk_p);
                        };
                        break;
                    case 2:
                        if (!P2done)
                        {
                            textUpcoming = "C(P" + (i + 1) + ") = " + resultText;
                        }
                        else
                        {
                            textUpcoming = "P2 ";
                            //textUpcoming = "B(P" + (i + 1) + ") ";
                            DrawCheckMark(sk_p);
                        };
                        break;
                    case 3:
                        if (!P3done)
                        {
                            textUpcoming = "C(P" + (i + 1) + ") = " + resultText;
                        }
                        else
                        {
                            textUpcoming = "P3 ";
                            //textUpcoming = "B(P" + (i + 1) + ") ";
                            DrawCheckMark(sk_p);

                        }; break;
                    case 4:
                        if (!P4done)
                        {
                            textUpcoming = "C(P" + (i + 1) + ") = " + resultText;
                        }
                        else
                        {
                            textUpcoming = "P4 ";
                            //textUpcoming = "B(P" + (i + 1) + ") ";
                            DrawCheckMark(sk_p);

                        }; break;
                    case 5:
                        if (!P5done)
                        {
                            textUpcoming = "C(P" + (i + 1) + ") = " + resultText;
                        }
                        else
                        {
                            textUpcoming = "P5 ";
                            //textUpcoming = "B(P" + (i + 1) + ") ";
                            DrawCheckMark(sk_p);

                        }; break;
                }
                canvas.DrawText(textUpcoming, sk_p, sk_blackText);

            }
        }

        /**********************************************************************
        *********************************************************************/
        public void DrawCell(SKCanvas canvas)
        {
            DrawCalculation();
            this.DrawBusyProcesses(canvas);
            this.DrawUpcomingProcesses(canvas);
            this.DrawDividingLine();

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

        private void DrawCalculation()
        {
            float starty = 0.21f;
            float y1Backg = 0.10f;
            float stepBackg = 0.20f;

            // vectors to display
            String oldVA = "";
            if (cellNumber == 0){oldVA = Deadlock.GetVectorA(); }
            else{ oldVA = Deadlock.GetHistory(cellNumber - 1, 1);}
            String currVB = vectorBProcesses[Int16.Parse(history)];
            String currVC = vectorCProcesses[Int16.Parse(history)];
            String currVA = vectorA;

            //Fromat text
            String resultTextVAold = "Aold   = ( ";
            String resultTextVB = "B(P" + history + ") = ( ";
            String resultTextVC = "C(P" + history + ") = ( ";
            String resultTextVAnew = "Anew = ( ";
            String space = "  ";
            for (int j = 0; j < currVB.Length; j++)
            {
                if (j == currVB.Length - 1) { space = " )"; }
                resultTextVB = resultTextVB + currVB[j] + space;
                resultTextVC = resultTextVC + currVC[j] + space;
                resultTextVAnew = resultTextVAnew + currVA[j] + space;
                resultTextVAold = resultTextVAold + oldVA[j] + space;
            }

            //Draw green background
            SKRect sk_rBackground0 = new SKRect(
                xStartBackgWindow1 ,
                yPercent(y1Backg),
                xStartBackgWindow1 + backgWindowLength,
                yPercent(y1Backg) + yPercent(stepBackg)); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground0, sk_BackgroundWhite); //left, top, right, bottom, color
            canvas.DrawRect(sk_rBackground0, sk_BackgroundGreen); //left, top, right, bottom, color

            // Aold
            SKPoint textPosition0 = new SKPoint(
                xStartBackgWindow1 +xText(0.02f),
                yPercent(starty));
            canvas.DrawText(resultTextVAold, textPosition0, sk_blackText);

            //Draw yellow background
            SKRect sk_rBackground = new SKRect(
                xStartBackgWindow1,
                yPercent(y1Backg) + yPercent(stepBackg),
                xStartBackgWindow1 + backgWindowLength,
                yPercent(y1Backg) + yPercent(stepBackg * 2)); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground, sk_BackgroundWhite); //left, top, right, bottom, color
            canvas.DrawRect(sk_rBackground, sk_BackgroundYellow); //left, top, right, bottom, color
            // C(x)

            SKPoint textPosition2 = new SKPoint(
                xStartBackgWindow1 + xText(0.02f),
                yPercent(starty) + yPercent(step));
            canvas.DrawText(resultTextVC, textPosition2, sk_blackText);

            //Draw red background
            SKRect sk_rBackground2 = new SKRect(
                xStartBackgWindow1,
                yPercent(y1Backg) + yPercent(stepBackg * 2),
                xStartBackgWindow1 + backgWindowLength,
                yPercent(y1Backg)+ yPercent(stepBackg * 3)); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground2, sk_BackgroundWhite); //left, top, right, bottom, color
            canvas.DrawRect(sk_rBackground2, sk_BackgroundRed); //left, top, right, bottom, color
            // B(x)
            SKPoint textPosition3 = new SKPoint(
                xStartBackgWindow1 + xText(0.02f),
                yPercent(starty) + yPercent(step * 2));
            canvas.DrawText(resultTextVB, textPosition3, sk_blackText);

            //Draw green background
            SKRect sk_rBackground4 = new SKRect(
                xStartBackgWindow1,
                yPercent(y1Backg) + yPercent(stepBackg * 3),
                xStartBackgWindow1 + backgWindowLength,
                yPercent(y1Backg) + yPercent(stepBackg * 4)); //left , top, right, bottom
            canvas.DrawRect(sk_rBackground4, sk_BackgroundWhite); //left, top, right, bottom, color
            canvas.DrawRect(sk_rBackground4, sk_BackgroundGreen); //left, top, right, bottom, color
            // Anew
            SKPoint textPosition4 = new SKPoint(
                xStartBackgWindow1 + xText(0.02f),
                yPercent(starty) + yPercent(step * 3));
            canvas.DrawText(resultTextVAnew, textPosition4, sk_blackText);
        }
        /**********************************************************************
        *********************************************************************/
        private void MakeSKPaint()
        {
            //black neutral text
            sk_blackText = new SKPaint
            {
                Color = SKColors.Black,
                //TextSize = yPercent(0.07f)
                TextSize = xText(textSize) //1.5, 15,

            };


            sk_blackTextSmaller = new SKPaint
            {
                Color = SKColors.Black,
                //TextSize = yPercent(0.07f)
                TextSize = xText(0.04f) //1.5, 15,

            };

            sk_BackgroundWhite = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                Color = new SKColor(255, 255, 255) //white
            };

            sk_Paint1 = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                Color = new SKColor(0, 0, 0) //black
            };

            sk_BackgroundBlue = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                Color = new SKColor(67, 110, 238).WithAlpha(30)
            };

            sk_BackgroundRed = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                Color = new SKColor(238, 130, 238).WithAlpha(30)
            };

            sk_BackgroundYellow = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                Color = new SKColor(255, 255, 0).WithAlpha(30)
            };

            sk_BackgroundGreen = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                Color = new SKColor(172, 255, 47).WithAlpha(30)
            };

            sk_CheckMarkContour = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                Color = SKColors.Green
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

            //other needed stuff
            textSize = 0.05f;
            step = 0.2f;
            backgWindowLength = xText(0.28f) + Deadlock.GetVectorC().Length * xText(0.05f);
            float xLengthLeftInCell = x2 - backgWindowLength * 3;
            spaceBetweenBackgWindows = xLengthLeftInCell / 4.0f;

            //TODO: should not be smaller than 0
            Debug.WriteLine(string.Format($" spaceBetweenBackgWindows: {spaceBetweenBackgWindows}"));

            xStartBackgWindow1 = spaceBetweenBackgWindows;
            xStartBackgWindow2 = xStartBackgWindow1 + backgWindowLength + spaceBetweenBackgWindows;
            xStartBackgWindow3 = xStartBackgWindow2 + backgWindowLength + spaceBetweenBackgWindows;

            Debug.WriteLine("t: " + Deadlock.GetTotalProcesses());
            backgWindowHeight = Deadlock.GetTotalProcesses() * yPercent(step);
            float yLengthLeftInCell = y2 - backgWindowHeight;
            yStartBackgWindow = yLengthLeftInCell/2.0f;
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
        static float xText(float p)
        {

            float percent = ((float)info.Height*(float)info.Width ) * (p/1000.0f);
            return percent;
        }
    }
}

