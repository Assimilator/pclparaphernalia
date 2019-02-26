﻿using System;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides common Tool functions.
    /// 
    /// © Chris Hutchinson 2013
    /// 
    /// </summary>

    public static class ToolCommonFunctions
    {
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d e c o m p o s e P a t h N a m e                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Break specified name down into component parts.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void decomposePathName (String pathName,
                                               ref String volName,
                                               ref String folderName,
                                               ref String lastName,
                                               ref String extension)
        {
            Int32 indxA,
                  indxB,
                  lenA,
                  lenB,
                  lenC;

            if (pathName == null)
            {
                volName    = "";
                folderName = "";
                lastName   = "";
                extension  = "";
            }
            else
            {
                lenA = pathName.Length;

                //------------------------------------------------------------//
                //                                                            //
                // Obtain volume (disk) name.                                 //
                //                                                            //
                //------------------------------------------------------------//

                indxA = pathName.IndexOf (":");

                if (indxA == -1)
                {
                    volName = "";
                }
                else
                {
                    volName = pathName.Substring (0, indxA + 1);
                }

                //------------------------------------------------------------//
                //                                                            //
                // Obtain folder name (including volume (disk) name).         //
                //                                                            //
                //------------------------------------------------------------//

                indxA = pathName.LastIndexOf ("\\");

                if (indxA == -1)
                {
                    folderName = "";
                }
                else
                {
                    folderName = pathName.Substring (0, indxA);
                }

                //------------------------------------------------------------//
                //                                                            //
                // Obtain last (terminal) name and extension name.            //
                //                                                            //
                //------------------------------------------------------------//

                lenB = lenA - indxA - 1;

                indxB = pathName.LastIndexOf (".", (lenA - 1), lenB);

                lenC = lenA - indxB - 1;

                if (indxB == -1)
                {
                    lastName  = pathName.Substring ((indxA + 1), lenB);
                    extension = "";
                }
                else
                {
                    lastName  = pathName.Substring ((indxA + 1),
                                                    (lenB - lenC - 1));
                    extension = pathName.Substring ((indxB + 1), lenC);
                }
            }
        }
        
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t F o l d e r N a m e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return folder name from supplied path name.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void getFolderName (String pathName,
                                          ref String folderName)
        {
            String tmpVol   = "", 
                   tmpTname = "",
                   tmpExt   = "";

            decomposePathName (pathName, ref tmpVol,
                               ref folderName, ref tmpTname, ref tmpExt);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T e r m i n a l N a m e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return terminal name (including extension) from supplied path      //
        // name.                                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void getTerminalName (String pathName,
                                            ref String fileName)
        {
            String tmpVol    = null,
                   tmpFolder = null,
                   tmpTname  = null,
                   tmpExt    = null;

            decomposePathName (pathName, ref tmpVol,
                               ref tmpFolder, ref tmpTname, ref tmpExt);

            fileName = tmpTname + "." + tmpExt;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s p l i t P a t h N a m e                                   I      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return folder and file names from supplied path name.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void splitPathName (String     pathName,
                                          ref String folderName,
                                          ref String fileName)
        {
            String tmpVol   = null,
                   tmpTname = null,
                   tmpExt   = null;

            decomposePathName (pathName, ref tmpVol,
                               ref folderName, ref tmpTname, ref tmpExt);

            fileName = tmpTname + "." + tmpExt;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s p l i t P a t h N a m e                                  I I     //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return folder and file names from supplied path name.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void splitPathName (String     pathName,
                                          ref String volName,
                                          ref String folderName,
                                          ref String fileName)
        {
            String tmpTname = null,
                   tmpExt = null;

            decomposePathName(pathName, ref volName,
                               ref folderName, ref tmpTname, ref tmpExt);

            fileName = tmpTname + "." + tmpExt;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s p l i t P a t h N a m e N o E x t                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return folder and file names from supplied path name, without      //
        // extension name.                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void splitPathNameNoExt (String pathName,
                                               ref String folderName,
                                               ref String fileName)
        {
            String tmpVol = null,
                   tmpExt = null;

            decomposePathName (pathName, ref tmpVol,
                               ref folderName, ref fileName, ref tmpExt);
        }
    }
}
