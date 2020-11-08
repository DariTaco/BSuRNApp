using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Collections.Generic;
using System.Diagnostics;

namespace WertheApp.BS
{
    public class BuddySystemViewCell : ViewCell
    {
        //VARIABLES
        private SKCanvasView skiaview;
        public static int dari; //determines backgroundcolor of the canvas

        private SKPaint sk_Paint1;
        private SKPaint sk_blackText, sk_blackTextSmall;
        private SKPaint sk_greenText;
        private SKPaint sk_redText;
        private SKPaint sk_processColor;
        private SKPaint sk_A, sk_B, sk_C, sk_D, sk_E, sK_F, sk_G, sk_H, sk_I,
        sk_J, sk_K, sk_L, sk_M, sk_N, sk_O, sk_P, sk_Q, sk_R, sk_S, sk_T, sk_U,
        sK_V, sk_W, sk_X, sk_Y, sk_Z;
        private String processName; //added or ended process
        public bool endProcess;

        private float xe, ye;

        private List<BuddySystemBlock> buddySystem;

        //CONSTRUCTOR
        /*TODO(get buddySystem List through constructor) constructor with parameters is somehow not possible. give buddySystem List to constructor*/
        public BuddySystemViewCell()
        {
            //note: List<BuddySystemBlock> buddySystem  = BuddySystem.buddySystem would not copy the list
            //note: buddySystem = new List<BuddySystemBlock>(BuddySystem.buddySystem); would not create new references
            //since the paintsurface method get's called several times, even after the object was created, 
            //there has to be a constant copy of the original list at the time
            //and since the upper way of copying the list keeps the same references to the original buddysystemblock objects
            //it has to be copied manually
            List<BuddySystemBlock> original = new List<BuddySystemBlock>(BuddySystem.buddySystem);

            buddySystem = new List<BuddySystemBlock>();
            for (int i = 0; i < original.Count; i++){

                BuddySystemBlock b = new BuddySystemBlock(
                    original[i].GetBlockSize(),
                    original[i].GetBuddyNo());

                if(original[i].GetFree() == false){
                    b.OccupyBlock(
                        original[i].GetProcessName(),
                        original[i].GetProcessSize());
                }
                buddySystem.Add(b);
             }

            this.processName = BuddySystem.currentProcess;
            this.endProcess = BuddySystem.endProcess;

            // crate the canvas
            skiaview = new SKCanvasView();
            skiaview.PaintSurface += PaintSurface;

            //changing background color for better clarity
            if (dari % 2 == 0)
            {
                skiaview.BackgroundColor = Color.FloralWhite;
            }
            else
            {
                skiaview.BackgroundColor = Color.WhiteSmoke;
            }
            dari++;

            // assign the canvas to the cell
            this.View = skiaview;
        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        // do the drawing
        //notice: this method get's called several times. For example when you scroll the listview in buddysystem or when the listview is updated
        void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            var surfaceWidth = info.Width;
            var surfaceHeight = info.Height;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            if (canvas != null)
            {
                canvas.Clear(); //Important! Otherwise the drawing will look messed up in iOS
            }

            /*important: the coordinate system starts in the upper left corner*/
            float lborder = canvas.LocalClipBounds.Left;
            float tborder = canvas.LocalClipBounds.Top;
            float rborder = canvas.LocalClipBounds.Right;
            float bborder = canvas.LocalClipBounds.Bottom;

            this.xe = rborder / 100; //using the variable surfacewidth instead would mess everything up
            this.ye = bborder / 100;

            MakeSKPaint(); //depends on xe and ye and therfore has to be called after they were initialized

            // draw on the canvas
            //rect for memory 80units long
            SKRect sk_r = new SKRect(xe * 2, ye * 10, xe * 82, ye * 90); //left, top, right, bottom
            canvas.DrawRect(sk_r, sk_Paint1); //draw an rect with dimensions of sk_r and paint sk_1

            //calculate units for memory
            float memoryUnit = (float)(80f / BuddySystem.absoluteMemorySize);

            //Draw blocks in memory
            float startValue = xe * 2;
            for (int i = 0; i < buddySystem.Count; i++)
            {
                //block
                int blockSize = buddySystem[i].GetBlockSize();
                float value = memoryUnit * blockSize;
                SKPoint sk_p1 = new SKPoint(startValue + xe * value, ye * 10);
                SKPoint sk_p2 = new SKPoint(startValue + xe * value, ye * 90);
                canvas.DrawLine(sk_p1, sk_p2, sk_Paint1);

                startValue = startValue + xe * value;

            }

            //draw processes in memory
            float startValue2 = xe * 2;
            for (int i = 0; i < buddySystem.Count; i++)
            {
                int blockSize2 = buddySystem[i].GetBlockSize();
                float value2 = memoryUnit * blockSize2;
                //process
                int processSize = buddySystem[i].GetProcessSize();
                float processValue = memoryUnit * processSize;
                bool isFree = buddySystem[i].GetFree();
                if (!isFree)
                {
                    //sry for that. Is there a better(shorter) solution?
                    SKPaint processColor;
                    switch(buddySystem[i].GetProcessName()){
                        case "A": 
                            processColor = sk_A;
                            break;
                        case "B":
                            processColor = sk_B;
                            break;
                        case "C":
                            processColor = sk_C;
                            break;
                        case "D":
                            processColor = sk_D;
                            break;
                        case "E":
                            processColor = sk_E;
                            break;
                        case "F":
                            processColor = sK_F;
                            break;
                        case "G":
                            processColor = sk_G;
                            break;
                        case "H":
                            processColor = sk_H;
                            break;
                        case "I":
                            processColor = sk_I;
                            break;
                        case "J":
                            processColor = sk_J;
                            break;
                        case "K":
                            processColor = sk_K;
                            break;
                        case "L":
                            processColor = sk_L;
                            break;
                        case "M":
                            processColor = sk_M;
                            break;
                        case "N":
                            processColor = sk_N;
                            break;
                        case "O":
                            processColor = sk_O;
                            break;
                        case "P":
                            processColor = sk_P;
                            break;
                        case "Q":
                            processColor = sk_Q;
                            break;
                        case "R":
                            processColor = sk_R;
                            break;
                        case "S":
                            processColor = sk_S;
                            break;
                        case "T":
                            processColor = sk_T;
                            break;
                        case "U":
                            processColor = sk_U;
                            break;
                        case "V":
                            processColor = sK_V;
                            break;
                        case "W":
                            processColor = sk_W;
                            break;
                        case "X":
                            processColor = sk_X;
                            break;
                        case "Y":
                            processColor = sk_Y;
                            break;
                        case "Z":
                            processColor = sk_Z;
                            break;
                        default:
                            processColor = sk_processColor;
                            break;
                    }

                    SKRect sk_r2 = new SKRect(startValue2, ye * 10 , startValue2 + xe * processValue , ye * 90);
                    canvas.DrawRect(sk_r2, processColor); //left, top, right, bottom, color
                    SKPoint textPosition = new SKPoint(startValue2 - xe + ((xe * processValue) / 2), ye * 60);
                    canvas.DrawText(buddySystem[i].GetProcessName(), textPosition, sk_blackTextSmall);
                }

                startValue2 = startValue2 + xe * value2;
            }

            //text
            SKPaint infoColor = sk_blackTextSmall;
            Debug.WriteLine("Processname " + this.processName);
            if (this.processName == "merge"){
                canvas.DrawText("merge", new SKPoint(xe * 84, ye * 55), infoColor);
            }else if(this.processName == "first"){
                canvas.DrawText("size: " + BuddySystem.absoluteMemorySize.ToString(), new SKPoint(xe * 84, ye * 55), infoColor); 
            }else{
                
                if(this.endProcess){
                    //TODO
                
                    canvas.DrawText("end: " + processName, new SKPoint(xe * 84, ye * 55), infoColor);
                }else{
                    //TODO
                    canvas.DrawText("start: " + processName, new SKPoint(xe * 84, ye * 55), infoColor);
                }
            }

            //execute all drawing actions
            canvas.Flush();
        }

        /**********************************************************************
        *********************************************************************/
        private void MakeSKPaint(){
            //create something to paint with (color, textsize, and even more....)
            sk_Paint1 = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                //IsStroke = true, //indicates whether to paint the stroke or the fill
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(0, 0, 0) //black
            };

            //process color
            sk_processColor = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                //IsStroke = true,

                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(4, 200, 0).WithAlpha(80) //rgb Transparency
            };

            //black neutral text
            sk_blackText = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye*30
            };

            //black neutral text
            sk_blackTextSmall = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = ye * 20
            };

            //green text for starting an process
            sk_greenText = new SKPaint
            {
                Color = SKColors.Green,
                TextSize = ye*30
            };

            //red text for ending an process
            sk_redText = new SKPaint
            {
                Color = SKColors.Red,
                TextSize = ye*30
            };

            //sk Paint for every single process name
            sk_A = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(238, 130, 238).WithAlpha(80)
            };

            sk_B = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(132,112,255).WithAlpha(80)
            };

            sk_C = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(64,224,208).WithAlpha(80)
            };

            sk_D = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(172,255,47).WithAlpha(80)
            };
            sk_E = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(255,215,0).WithAlpha(80)
            };

            sK_F = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(255,105,180).WithAlpha(80)
            };

            sk_G = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(67,110,238).WithAlpha(80)
            };

            sk_H = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(255,255,0).WithAlpha(80)
            };

            sk_I = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(238, 130, 238).WithAlpha(80)
            };

            sk_J = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(132, 112, 255).WithAlpha(80)
            };

            sk_K = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(64, 224, 208).WithAlpha(80)
            };

            sk_L = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(172, 255, 47).WithAlpha(80)
            };

            sk_M = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(255, 215, 0).WithAlpha(80)
            };

            sk_N = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(255, 105, 180).WithAlpha(80)
            };

            sk_O = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(67, 110, 238).WithAlpha(80)
            };

            sk_P = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(255, 255, 0).WithAlpha(80)
            };

            sk_Q = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(238, 130, 238).WithAlpha(80)
            };

            sk_R = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(132, 112, 255).WithAlpha(80)
            };

            sk_S = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(64, 224, 208).WithAlpha(80)
            };

            sk_T = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(172, 255, 47).WithAlpha(80)
            };

            sk_U = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(255, 215, 0).WithAlpha(80)
            };

            sK_V = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(255, 105, 180).WithAlpha(80)
            };

            sk_W = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(67, 110, 238).WithAlpha(80)
            };

            sk_X = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(255, 255, 0).WithAlpha(80)
            };
            sk_Y = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(238, 130, 238).WithAlpha(80)
            };

            sk_Z = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = new SKColor(132, 112, 255).WithAlpha(80)
            };
        }
    }
}
