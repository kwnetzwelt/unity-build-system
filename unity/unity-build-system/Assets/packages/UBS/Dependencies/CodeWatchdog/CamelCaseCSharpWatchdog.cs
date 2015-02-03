// This file is part of CodeWatchdog.
// https://bitbucket.org/flberger/codewatchdog

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CodeWatchdog
{
    /// <summary>
    /// A Watchdog implementation for C# that favors camelCase coding style.
    /// </summary>
    public class CamelCaseCSharpWatchdog: Watchdog
    {
        // Error code variables, for reading convenience
        //
        const int TAB_ERROR = 0; 
        const int PASCALCASE_ERROR = 1;
        const int CAMELCASE_ERROR = 2;
        const int SPECIALCHARACTER_ERROR = 3;
        const int MISSINGBRACES_ERROR = 4;
        const int MULTIPLESTATEMENT_ERROR = 5;
        const int COMMENTONSAMELINE_ERROR = 6;
        const int COMMENTNOSPACE_ERROR = 7;
        const int CLASSNOTDOCUMENTED_ERROR = 8;
        
        // MSDN Coding Conventions
        // https://msdn.microsoft.com/en-us/library/ff926074.aspx
        // https://msdn.microsoft.com/en-us/library/xzf533w0%28v=vs.71%29.aspx
        //
        // TODO: camelCase for parameters
        // TODO: Use implicit typing with var in for, foreach
        // TODO: Prefer 'using' to 'try ... finally' for cleanups
        
        // Selected antipatterns
        //
        // TODO: *** if ()- nullchecks without else
        
        // C#-Workshop
        //
        // TODO: .Equals() statt ==
        // TODO: int parse mit invariant culture
        
        // Internal Unity C# Coding Conventions:
        //
        // TODO: *** Properties beginnen mit einem großen Buchstaben
        // TODO: *** Funktionen/Methoden und Klassen beginnen mit einem großen Buchstaben
        // TODO: *** Parameter von Funktionen/Methoden beginnen mit einem kleinen Buchstaben
        // TODO: *** Enums werden in CamelCase Schreibweise (beginnend mit einem Großbuchstaben) benannt
        // TODO: *** Interfaces beginnen mit einem großen "I"
                
        // c2 Code Smells: http://www.c2.com/cgi/wiki?CodeSmell
        //
        // TODO: Duplicated code
        // TODO: Method too big
        // TODO: Classes with too many variables
        // TODO: Classes with too little variables
        // TODO: Classes with too much code
        // TODO: Classes with too little code
        // TODO: Too many private methods
        // TODO: Empty catch clauses
        // TODO: Long method names
        // TODO: Too many parameters
        // TODO: Deeply nested if clauses / loops
        
        /// <summary>
        /// Initialise the underlying Watchdog for C#.
        /// </summary>
        public override void Init()
        {
            base.Init();
            
            STATEMENT_DELIMTER = char.Parse(";");
            START_BLOCK_DELIMITER = char.Parse("{");
            END_BLOCK_DELIMITER = char.Parse("}");
            STRING_DELIMITERS = new List<char>() {char.Parse("\"")};
            STRING_ESCAPE = char.Parse("\\");
            START_COMMENT_DELIMITER = "//";
            END_COMMENT_DELIMITER = "\n";
            
            ErrorCodeStrings = new Dictionary<int, string>();
            
            ErrorCodeStrings[TAB_ERROR] = "Tabs instead of spaces used for indentation";
            ErrorCodeStrings[PASCALCASE_ERROR] = "Identifier is not in PascalCase";
            ErrorCodeStrings[CAMELCASE_ERROR] = "Identifier is not in camelCase";
            ErrorCodeStrings[SPECIALCHARACTER_ERROR] = "Disallowed character used in identifier";
            ErrorCodeStrings[MISSINGBRACES_ERROR] = "Missing curly braces in if / while / foreach / for";
            ErrorCodeStrings[MULTIPLESTATEMENT_ERROR] = "Multiple statements on a single line";
            ErrorCodeStrings[COMMENTONSAMELINE_ERROR] = "Comment not on a separate line";
            ErrorCodeStrings[COMMENTNOSPACE_ERROR] = "No space between comment delimiter and comment text";
            ErrorCodeStrings[CLASSNOTDOCUMENTED_ERROR] = "Class not documented";
            
            StatementHandler += CheckStatement;
            CommentHandler += CheckComment;
            StartBlockHandler += CheckStartBlock;
        }
        
        /// <summary>
        /// Check a single statement.
        /// </summary>
        /// <param name="statement">A string containing a statement, possibly multi-line.</param>
        void CheckStatement(string statement)
        {
            // TODO: *** /// comment public members
            
            // TODO: *** 4-character indents
            
            // TODO: Use var for common types and new statements.
            
            // TAB_ERROR
            //
            if (statement.Contains("\t"))
            {
                if (ErrorCodeCount.ContainsKey(TAB_ERROR))
                {
                    ErrorCodeCount[TAB_ERROR] += 1;
                }
                else
                {
                    ErrorCodeCount[TAB_ERROR] = 1;
                }
                
                // TODO: The line report is inaccurate, as several lines may have passed.
                // HACK: Assuming the next line and using CheckedLinesOfCode + 1.
                //
                Woff(string.Format("{0} (line {1})", ErrorCodeStrings[TAB_ERROR], CheckedLinesThisFile + 1));
            }
            
            // MULTIPLESTATEMENT_ERROR
            //
            // Trim leading spaces before check
            //
            if (CheckedLinesThisFile > 1 && !statement.TrimStart(char.Parse(" ")).StartsWith("\n"))
            {
                if (ErrorCodeCount.ContainsKey(MULTIPLESTATEMENT_ERROR))
                {
                    ErrorCodeCount[MULTIPLESTATEMENT_ERROR] += 1;
                }
                else
                {
                    ErrorCodeCount[MULTIPLESTATEMENT_ERROR] = 1;
                }
                
                Woff(string.Format("{0} (line {1})", ErrorCodeStrings[MULTIPLESTATEMENT_ERROR], CheckedLinesThisFile + 1));
            }
            
            // Identifiers
            //
            string possibleIdentifier = "";
            
            Match firstMatch = Regex.Match(statement, @"\s+\w+(<[\w, ]+>)?\s+(\w+)\s*$");
           
            if (firstMatch.Success)
            {
                possibleIdentifier = firstMatch.Groups[2].Value;
                
                Logging.Debug("Match: " + possibleIdentifier);
            }
            else
            {
                Match secondMatch = Regex.Match(statement, @"\s+\w+(<[\w, ]+>)?\s+(\w+)\s*=");
                
                if (secondMatch.Success)
                {
                    possibleIdentifier = secondMatch.Groups[2].Value;
                    
                    Logging.Debug("Match: " + possibleIdentifier);
                }
            }
            
            if (possibleIdentifier != "")
            {
                // TODO: Identifiers should not contain common types. But this is hard to check, as 'Char' or 'Int' may be legitimate in 'Charter' or 'International'.
                
                // SPECIALCHARACTER_ERROR
                //
                if (possibleIdentifier.Contains("_"))
                {
                    if (ErrorCodeCount.ContainsKey(SPECIALCHARACTER_ERROR))
                    {
                        ErrorCodeCount[SPECIALCHARACTER_ERROR] += 1;
                    }
                    else
                    {
                        ErrorCodeCount[SPECIALCHARACTER_ERROR] = 1;
                    }
                    
                    // TODO: The line report is inaccurate, as several lines may have passed.
                    // HACK: Assuming the next line and using CheckedLinesOfCode + 1.
                    //
                    Woff(string.Format("{0}: '{1}' (line {2})", ErrorCodeStrings[SPECIALCHARACTER_ERROR], possibleIdentifier, CheckedLinesThisFile + 1));
                }
                else
                {
                    if (statement.Contains("const "))
                    {
                        // PASCALCASE_ERROR
                        // TODO: Check for more PascalCase / camelCase characteristics
                        //
                        if (possibleIdentifier.Length > 2 && char.IsLower(possibleIdentifier, 0))
                        {
                            if (ErrorCodeCount.ContainsKey(PASCALCASE_ERROR))
                            {
                                ErrorCodeCount[PASCALCASE_ERROR] += 1;
                            }
                            else
                            {
                                ErrorCodeCount[PASCALCASE_ERROR] = 1;
                            }
                            
                            // TODO: The line report is inaccurate, as several lines may have passed.
                            // HACK: Assuming the next line and using CheckedLinesOfCode + 1.
                            //
                            Woff(string.Format("{0}: '{1}' (line {2})", ErrorCodeStrings[PASCALCASE_ERROR], possibleIdentifier, CheckedLinesThisFile + 1));
                        }
                    }
                    else
                    {
                        // CAMELCASE_ERROR
                        // TODO: Check for more PascalCase / camelCase characteristics
                        //
                        if (possibleIdentifier.Length > 2 && char.IsUpper(possibleIdentifier, 0))
                        {
                            
                            if (ErrorCodeCount.ContainsKey(CAMELCASE_ERROR))
                            {
                                ErrorCodeCount[CAMELCASE_ERROR] += 1;
                            }
                            else
                            {
                                ErrorCodeCount[CAMELCASE_ERROR] = 1;
                            }
                            
                            // TODO: The line report is inaccurate, as several lines may have passed.
                            // HACK: Assuming the next line and using CheckedLinesOfCode + 1.
                            //
                            Woff(string.Format("{0}: '{1}' (line {2})", ErrorCodeStrings[CAMELCASE_ERROR], possibleIdentifier, CheckedLinesThisFile + 1));
                        }
                    }
                }
            }
            
            // MISSINGBRACES_ERROR
            //
            if (statement.Trim().StartsWith("if")
                || statement.Trim().StartsWith("while")
                || statement.Trim().StartsWith("foreach")
                || statement.Trim().StartsWith("for"))
            {
                if (ErrorCodeCount.ContainsKey(MISSINGBRACES_ERROR))
                {
                    ErrorCodeCount[MISSINGBRACES_ERROR] += 1;
                }
                else
                {
                    ErrorCodeCount[MISSINGBRACES_ERROR] = 1;
                }
                
                // TODO: The line report is inaccurate, as several lines may have passed.
                // HACK: Assuming the next line and using CheckedLinesOfCode + 1.
                //
                Woff(string.Format("{0} (line {1})", ErrorCodeStrings[MISSINGBRACES_ERROR], CheckedLinesThisFile + 1));
            }
            
            return;
        }
        
        /// <summary>
        /// Check a single comment.
        /// </summary>
        /// <param name="comment">A string containing a comment, possibly multi-line.</param>
        void CheckComment(string comment, string precedingInput)
        {
            // TODO: Begin comments with uppercase letter
            // TODO: End comment with a period.

            // COMMENTONSAMELINE_ERROR
            //
            if (CheckedLinesThisFile > 1 && !precedingInput.Contains("\n"))
            {
                if (ErrorCodeCount.ContainsKey(COMMENTONSAMELINE_ERROR))
                {
                    ErrorCodeCount[COMMENTONSAMELINE_ERROR] += 1;
                }
                else
                {
                    ErrorCodeCount[COMMENTONSAMELINE_ERROR] = 1;
                }
                
                Woff(string.Format("{0} (line {1})", ErrorCodeStrings[COMMENTONSAMELINE_ERROR], CheckedLinesThisFile));
            }
            
            // COMMENTNOSPACE_ERROR
            // Also include /// doc comments.
            //
            if (!(comment.StartsWith(START_COMMENT_DELIMITER + " ") || comment.StartsWith(START_COMMENT_DELIMITER + "/ ")))
            {
                if (ErrorCodeCount.ContainsKey(COMMENTNOSPACE_ERROR))
                {
                    ErrorCodeCount[COMMENTNOSPACE_ERROR] += 1;
                }
                else
                {
                    ErrorCodeCount[COMMENTNOSPACE_ERROR] = 1;
                }
                
                Woff(string.Format("{0} (line {1})", ErrorCodeStrings[COMMENTNOSPACE_ERROR], CheckedLinesThisFile));
            }
        }
        
        /// <summary>
        /// Checks the beginning of a block.
        /// </summary>
        /// <param name="startBlock">A string containing the start block.</param>
        void CheckStartBlock(string startBlock)
        {
            // TODO: *** /// comment methods
            
            // CLASSNOTDOCUMENTED_ERROR
            //
            if (startBlock.Contains("public ")
                && startBlock.Contains(" class ")
                && !PreviousToken.Contains(START_COMMENT_DELIMITER + "/")
                && !PreviousToken.Contains("</summary>"))
            {
                if (ErrorCodeCount.ContainsKey(CLASSNOTDOCUMENTED_ERROR))
                {
                    ErrorCodeCount[CLASSNOTDOCUMENTED_ERROR] += 1;
                }
                else
                {
                    ErrorCodeCount[CLASSNOTDOCUMENTED_ERROR] = 1;
                }
                
                Woff(string.Format("{0} (line {1})", ErrorCodeStrings[CLASSNOTDOCUMENTED_ERROR], CheckedLinesThisFile));
            }
            
            return;
        }
    }
}

