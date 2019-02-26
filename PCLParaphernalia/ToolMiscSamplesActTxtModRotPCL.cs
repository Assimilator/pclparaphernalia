﻿using System;
using System.IO;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides PCL support for the Rotation element
    /// of the Text Modification action of the MiscSamples tool.
    /// 
    /// © Chris Hutchinson 2014
    /// 
    /// </summary>

    static class ToolMiscSamplesActTxtModRotPCL
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const Int32 _macroId            = 1;
        const UInt16 _unitsPerInch      = PCLWriter.sessionUPI;
        const UInt16 _plotUnitsPerInch  = PCLWriter.plotterUnitsPerInchHPGL2;

        const Int16 _pageOriginX        = (_unitsPerInch * 1);
        const Int16 _pageOriginY        = (_unitsPerInch * 1);
        const Int16 _incInch            = (_unitsPerInch * 1);
        const Int16 _lineInc            = (_unitsPerInch * 5) / 6;

        const Int16 _posXDesc           = _pageOriginX;
        const Int16 _posXData1          = _pageOriginX + ((9 * _incInch) / 2);
        const Int16 _posXData2          = _pageOriginX + ((9 * _incInch) / 2);

        const Int16 _posYHddr           = _pageOriginY;
        const Int16 _posYDesc1          = _pageOriginY + (1 * _incInch);
        const Int16 _posYDesc2          = _pageOriginY + (5 * _incInch);
        const Int16 _posYData1          = _pageOriginY + (3 * _incInch);
        const Int16 _posYData2          = _pageOriginY + (7 * _incInch);

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Static variables.                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//
            
        static Int32 _indxFontArial     = PCLFonts.getIndexForName("Arial");
        static Int32 _indxFontCourier   = PCLFonts.getIndexForName("Courier");

        static Int32 _logPageWidth;
        static Int32 _logPageHeight;
        static Int32 _paperWidth;
        static Int32 _paperHeight;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e J o b                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate test data.                                                //
        //                                                                    //
        // Some sequences are built up as (Unicode) strings, then converted   //
        // to byte arrays before writing out - this works OK because all the  //
        // characters we're using are within the ASCII range (0x00-0x7f) and  //
        // are hence represented using a single byte in the UTF-8 encoding.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void generateJob(BinaryWriter prnWriter,
                                       Int32        indxPaperSize,
                                       Int32        indxPaperType,
                                       Int32        indxOrientation,
                                       Boolean      formAsMacro)
        {
            PCLOrientations.eAspect aspect;

            UInt16 logXOffset;

            //----------------------------------------------------------------//

            aspect = PCLOrientations.getAspect(indxOrientation);

            logXOffset = PCLPaperSizes.getLogicalOffset(indxPaperSize,
                                                        _unitsPerInch, aspect);

            _logPageWidth = PCLPaperSizes.getLogPageWidth (indxPaperSize,
                                                           _unitsPerInch,
                                                           aspect);

            _logPageHeight = PCLPaperSizes.getLogPageLength (indxPaperSize,
                                                          _unitsPerInch,
                                                          aspect);

            _paperWidth = PCLPaperSizes.getPaperWidth (indxPaperSize,
                                                       _unitsPerInch,
                                                       aspect);

            _paperHeight = PCLPaperSizes.getPaperLength (indxPaperSize,
                                                         _unitsPerInch,
                                                         aspect);

            //----------------------------------------------------------------//

            generateJobHeader(prnWriter,
                              indxPaperSize,
                              indxPaperType,
                              indxOrientation,
                              formAsMacro,
                              logXOffset);

            generatePage(prnWriter,
                         indxPaperSize,
                         indxPaperType,
                         indxOrientation,
                         formAsMacro,
                         logXOffset);

            generateJobTrailer(prnWriter, formAsMacro);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e J o b H e a d e r                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write stream initialisation sequences to output file.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateJobHeader(BinaryWriter prnWriter,
                                              Int32        indxPaperSize,
                                              Int32        indxPaperType,
                                              Int32        indxOrientation,
                                              Boolean      formAsMacro,
                                              UInt16       logXOffset)
        {
            PCLWriter.stdJobHeader(prnWriter, "");

            if (formAsMacro)
                generateOverlay(prnWriter, true, logXOffset,
                                indxPaperSize, indxOrientation);

            PCLWriter.pageHeader(prnWriter,
                                 indxPaperSize,
                                 indxPaperType,
                                 indxOrientation,
                                 PCLPlexModes.eSimplex);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e J o b T r a i l e r                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write tray map termination sequences to output file.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateJobTrailer(BinaryWriter prnWriter,
                                               Boolean      formAsMacro)
        {
            PCLWriter.stdJobTrailer(prnWriter, formAsMacro, _macroId);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e O v e r l a y                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write background data sequences to output file.                    //
        // Optionally top and tail these with macro (user-defined stream)     //
        // definition sequences.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateOverlay(BinaryWriter prnWriter,
                                            Boolean      formAsMacro,
                                            UInt16       logXOffset,
                                            Int32        indxPaperSize,
                                            Int32        indxOrientation)
        {
            Int16 posX,
                  posY;

            Int16 ptSize;

            Int16 boxX,
                  boxY,
                  boxHeight,
                  boxWidth;

            Byte stroke = 1;

            //----------------------------------------------------------------//
            //                                                                //
            // Header                                                         //
            //                                                                //
            //----------------------------------------------------------------//

            if (formAsMacro)
                PCLWriter.macroControl(prnWriter, _macroId,
                                  PCLWriter.eMacroControl.StartDef);

            //----------------------------------------------------------------//
            //                                                                //
            // Box.                                                           //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.patternSet(prnWriter,
                                  PCLWriter.ePatternType.Shading,
                                  60);

            boxX = (Int16) ((_unitsPerInch / 2) - logXOffset);
            boxY = (Int16) (_unitsPerInch / 2);

            boxWidth  = (Int16) (_paperWidth  - _unitsPerInch);
            boxHeight = (Int16) (_paperHeight - _unitsPerInch);

            PCLWriter.rectangleOutline (prnWriter, boxX, boxY,
                                        boxHeight, boxWidth, stroke,
                                        false, false);

            //----------------------------------------------------------------//
            //                                                                //
            // Text.                                                          //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.patternSet(prnWriter,
                                  PCLWriter.ePatternType.SolidBlack,
                                  0);

            ptSize  = 15;

            PCLWriter.font(prnWriter, true, "19U",
                           PCLFonts.getPCLFontSelect(_indxFontCourier,
                                                      PCLFonts.eVariant.Bold,
                                                      ptSize, 0));

            posX = (Int16)(_posXDesc - logXOffset);
            posY = _posYHddr;

            PCLWriter.text(prnWriter, posX, posY, 0, 
                      "PCL & HP-GL/2 Text Rotation:");

            ptSize  = 12;

            PCLWriter.font(prnWriter, true, "19U",
                           PCLFonts.getPCLFontSelect(_indxFontCourier,
                                                      PCLFonts.eVariant.Regular,
                                                      ptSize, 0));

            posY = _posYDesc1;

            PCLWriter.text(prnWriter, posX, posY, 0, 
                      "Orthogonal:");

            posY = _posYDesc2;

            PCLWriter.text(prnWriter, posX, posY, 0, 
                      "Non-orthogonal:");
            

            //----------------------------------------------------------------//
            //                                                                //
            // Overlay end.                                                   //
            //                                                                //
            //----------------------------------------------------------------//

            PCLWriter.patternSet (prnWriter,
                                  PCLWriter.ePatternType.SolidBlack,
                                  0);

            if (formAsMacro)
                PCLWriter.macroControl(prnWriter, 0,
                                       PCLWriter.eMacroControl.StopDef);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e P a g e                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write individual test data page sequences to output file.          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generatePage(BinaryWriter prnWriter,
                                         Int32        indxPaperSize,
                                         Int32        indxPaperType,
                                         Int32        indxOrientation,
                                         Boolean      formAsMacro,
                                         UInt16       logXOffset)
        {
            String lbCRTerm    = "\x0d" + "~";

            Int16 posX,
                  posY;

            Int16 ptSize,
                  degrees;

            Int16 boxX,
                  boxY,
                  boxHeight,
                  boxWidth;

            Double scaleX,
                   scaleY;

            Double angle,
                   sinAngle,
                   cosAngle;

            //----------------------------------------------------------------//
            
            if (formAsMacro)
                PCLWriter.macroControl(prnWriter, _macroId,
                                       PCLWriter.eMacroControl.Call);
            else
                generateOverlay(prnWriter, false, logXOffset,
                                indxPaperSize, indxOrientation);

            //----------------------------------------------------------------//
            //                                                                //
            // HP-GL/2 picture frame and initialisation.                      //
            //                                                                //
            // Plotter units are always absolute at 0.025mm (1/1016 inch),    //
            // but many HP-GL/2 commands use (definable) user units.          //
            // It makes the code clearer if we use the same units in HP-GL/2  //
            // as we do in PCL, so the SC (scale) command is used to set      //
            // user-units to 600 units-per-inch.                              //
            //                                                                //
            // Note that the default HP-GL/2 Y-axis has its origin at         //
            // lower-left of the picture frame, and Y-coordinate values       //
            // increase UP the page, whilst the PCL Y-axis has its origin at  //
            // the top margin and Y-coordinate values increase DOWN the page. // 
            //                                                                //
            // It is possible to use the same (600 upi) coordinates as PCL by //
            // using:                                                         //
            //  SC0,1.6933,0,-1.6933,2                                        // 
            //  IR0,100,100,0                                                 //   
            // Note that the IR coordinates shown in the example in the "PCL  //
            // Technical Reference" manual are different and are incorrect!   //
            // One drawback to using the same origin and axis direction is    //
            // that some commands (such as SR) then have to use negative      //
            // Y-values to avoid mirroring.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            scaleX = (Double)_plotUnitsPerInch / _unitsPerInch;
            scaleY = (Double)_plotUnitsPerInch / _unitsPerInch;

            boxX = 0;
            boxY = 0;
            boxWidth  = (Int16)(_logPageWidth);
            boxHeight = (Int16)(_logPageHeight);

            PCLWriter.pictureFrame (prnWriter,
                                    boxX,
                                    boxY,
                                    boxHeight,
                                    boxWidth);

            PCLWriter.modeHPGL2 (prnWriter, false, false);

            PCLWriter.cmdHPGL2 (prnWriter, "IN", "", false);
            PCLWriter.cmdHPGL2 (prnWriter, "SP", "1", true);
            PCLWriter.cmdHPGL2 (prnWriter, "DT", "~", false);

            PCLWriter.cmdHPGL2(prnWriter, "SC",
                                "0," + scaleX.ToString("F4") +
                                ",0," + (-scaleY).ToString("F4") +
                                ",2",
                                false);

            PCLWriter.cmdHPGL2 (prnWriter, "IR", "0,100,100,0", false);
            PCLWriter.cmdHPGL2 (prnWriter, "PU", "0,0", true);

            PCLWriter.modePCL (prnWriter, true);

            //----------------------------------------------------------------//
            //                                                                //
            // Rotated text.                                                  //
            //                                                                //
            //----------------------------------------------------------------//

            ptSize = 36;

            PCLWriter.font(prnWriter, true, "19U",
                           PCLFonts.getPCLFontSelect(_indxFontArial,
                                                      PCLFonts.eVariant.Regular,
                                                      ptSize, 0));

            //----------------------------------------------------------------//
            // Orthogonal text                                                //
            //----------------------------------------------------------------//

            posX = (Int16)(_posXData1 - logXOffset);
            posY = _posYData1;

            posX += _lineInc / 4;

            PCLWriter.printDirection(prnWriter, 0);

            PCLWriter.textRotated (prnWriter, posX, posY, 0, 0, true,
                                   "angle 0");

            posX -= _lineInc / 2;

            PCLWriter.textRotated (prnWriter, posX, posY, 0, 180, true,
                                   "ccw 180");

            posX += _lineInc / 4;
            posY -= _lineInc / 4;

            PCLWriter.textRotated (prnWriter, posX, posY, 0, 90, true,
                                   "ccw 90");

            posY += _lineInc / 2;

            PCLWriter.textRotated (prnWriter, posX, posY, 0, 270, true,
                                   "ccw 270");

            //----------------------------------------------------------------//
            // direction quadrants I and III                                  //
            //----------------------------------------------------------------//

            posX = (Int16)(_posXData2 - logXOffset);
            posY = _posYData2;

            PCLWriter.modeHPGL2(prnWriter, false, false);

            PCLWriter.cmdHPGL2(prnWriter, "SD",
                                PCLFonts.getHPGL2FontDef(_indxFontArial,
                                       PCLFonts.eVariant.Regular,
                                       14, ptSize, 0),
                                true);

            PCLWriter.cmdHPGL2(prnWriter, "PA",
                                posX.ToString() + "," +
                                posY.ToString(), false);

            degrees = 30;
            angle = Math.PI * degrees / 180.0;
            sinAngle = Math.Sin(angle);
            cosAngle = Math.Cos(angle);

            PCLWriter.cmdHPGL2(prnWriter, "DI",
                                cosAngle.ToString() + "," +
                                sinAngle.ToString(), false);

            PCLWriter.cmdHPGL2(prnWriter, "LB",
                               "angle +30" + lbCRTerm, true);

            PCLWriter.cmdHPGL2(prnWriter, "DI",
                                "-" + cosAngle.ToString() + "," +
                                "-" + sinAngle.ToString(), false);

            PCLWriter.cmdHPGL2(prnWriter, "LB",
                               "angle +210" + lbCRTerm, true);

            //----------------------------------------------------------------//
            // direction quadrants II and IV                                  //
            //----------------------------------------------------------------//

            posX = (Int16)(_posXData2 - logXOffset - ((1 * _incInch) / 4));

            PCLWriter.modeHPGL2(prnWriter, false, false);

            PCLWriter.cmdHPGL2(prnWriter, "SD",
                                PCLFonts.getHPGL2FontDef(_indxFontArial,
                                       PCLFonts.eVariant.Regular,
                                       14, ptSize, 0),
                                true);

            PCLWriter.cmdHPGL2(prnWriter, "PA",
                                posX.ToString() + "," +
                                posY.ToString(), false);

            degrees = 45;
            angle    = Math.PI * degrees / 180.0;
            sinAngle = Math.Sin(angle);
            cosAngle = Math.Cos(angle);

            PCLWriter.cmdHPGL2(prnWriter, "DI",
                                "-" + cosAngle.ToString() + "," +
                                sinAngle.ToString(), false);

            PCLWriter.cmdHPGL2(prnWriter, "LB",
                               "angle +135" + lbCRTerm, true);

            posX += ((1 * _incInch) / 2);
            posY += ((_lineInc * 1) / 6);

            PCLWriter.cmdHPGL2(prnWriter, "PA",
                                posX.ToString() + "," +
                                posY.ToString(), false);

            PCLWriter.cmdHPGL2(prnWriter, "DI",
                                cosAngle.ToString() + "," +
                                "-" + sinAngle.ToString(), false);

            PCLWriter.cmdHPGL2(prnWriter, "LB",
                               "angle +315" + lbCRTerm, true);

            //----------------------------------------------------------------//
            // resets                                                         //
            //----------------------------------------------------------------//

            posX = (Int16)(_posXData2 - logXOffset);
            posY += ((_lineInc * 1) / 2);

            PCLWriter.cmdHPGL2(prnWriter, "DI", "1,0", false);

            //----------------------------------------------------------------//

            PCLWriter.formFeed(prnWriter);
        }
    }
}
