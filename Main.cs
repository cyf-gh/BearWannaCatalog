using AppKit;

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BWC_Mac {
    static class MainClass {
        static void Main( string[] args ) {
            ProcVersion( args );
            ProcArgHelp( args );
            ProcTwoDirs( args );
            ProcOneDir( args );
            // NSApplication.Init();
            // NSApplication.Main( args );
        }
        static void ProcVersion( string[] args ) {
            if ( args.Length == 1 ) {
                if ( args[0] == "-ver" ) {
                    Console.WriteLine( "Version: 0.010" );
                }
            }
        }
        static void ProcArgHelp( string[] args ) {
            if ( args.Length == 1 ) {
                if ( args[0] == "--help" ) {
                    Console.WriteLine( "Attention: Obey Bear's rule and put the tag at the END OF FILE!" );
                    Console.WriteLine( "\t[-d dir1]\t[Catalog dir 1 and put the catalog result here.]" );
                    Console.WriteLine( "\t[-st dir1 dir2]\t[Catalog dir1 and put the catalog result in dir2.]" );
                    Console.WriteLine( "\t[-ver] \tShow Current Version." );
                }
            }
        }
        static void ProcOneDir( string[] args ) {
            if ( args.Length == 2 ) {
                if ( args[0] == "-d" ) {
                    CatalogByTags( args[1], args[1] );
                }
            }
        }
        static void ProcTwoDirs( string[] args ) {
            if ( args.Length == 3 ) {
                if ( args[0] == "-st" ) {
                    CatalogByTags( args[1], args[2] );
                }
            }
        }
        static void CatalogByTags( string srcDir, string targDir ) {
            string PathSrc = srcDir;
            string PathTarget = targDir;
            // Make directorys sure.
            if ( !Directory.Exists( PathSrc ) ) {
                Console.WriteLine( "Directory: " + PathSrc + "Do not exsits.\n Cataloging Failed" );
                System.Environment.Exit( -1 );
            }
            if ( !Directory.Exists( PathTarget ) ) {
                Directory.CreateDirectory( PathTarget );
            }

            DirectoryInfo DirSrcInfo = new DirectoryInfo( srcDir );
            foreach ( var md in DirSrcInfo.GetFiles() ) {
                if ( md.Extension != ".md" ) {
                    continue;
                }
                try {
                    StreamReader reader = new StreamReader( md.OpenRead(), Encoding.UTF8 );
                    string text = reader.ReadToEnd();
                    reader.Close();

                    text = Regex.Replace( text, "```([\\s\\S]*?)```[\\s]?", "" );
                    text = Regex.Replace( text, "`{1,2}[^`](.*?)`{1,2}\n", "" );
                    text = Regex.Replace( text, "\n(&gt;|\\>)(.*)", "" );

                    Match match1;
                    MatchCollection match = Regex.Matches( text, "(?<=#)([^\\s#]{1})([^#\\n]+)(?=#)" );
                    if ( match.Count != 0 ) {
                        match1 = match[match.Count - 1];
                    } else {
                        match = Regex.Matches( text, "(?<=#)([^#\\s]{1,})(?=\\s|$)" );
                        if ( match.Count != 0 ) {
                            match1 = match[match.Count - 1];
                        } else {
                            continue;
                        }
                    }

                    string FullTagDirectory = Path.Combine( targDir, match1.ToString() );
                    if ( !Directory.Exists( FullTagDirectory ) ) {
                        Directory.CreateDirectory( FullTagDirectory );
                    }
                    FullTagDirectory = Path.Combine( FullTagDirectory, md.Name );
                    if ( md.DirectoryName != FullTagDirectory ) {
                        File.Move( md.FullName, FullTagDirectory );
                    }
                } catch ( Exception ex ) {
                    Console.WriteLine( "Error: " + ex.Message );
                    System.Environment.Exit( -1 );
                }
            }
        }
    }
}
