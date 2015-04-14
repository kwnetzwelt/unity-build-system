// This file is part of CodeWatchdog.
// https://bitbucket.org/flberger/codewatchdog

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UBSCodeWatchdog
{
    /// <summary>
    /// A Watchdog implementation for C# that favors camelCase coding style.
    /// </summary>
    public class CamelCaseCSharpWatchdog: UBSCodeWatchdog.Watchdog
    {
        // Error code variables, for reading convenience
        //
        const int TabError = 0; 
        const int PascalCaseError = 1;
        const int CamelCaseError = 2;
        const int SpecialCharacterError = 3;
        const int MissingBracesError = 4;
        const int MultipleStatementError = 5;
        const int CommentOnSameLineError = 6;
        const int CommentNoSpaceError = 7;
        const int ClassNotDocumentedError = 8;
        const int MethodNotDocumentedError = 9;
        const int InterfaceNamingError = 10;

        /// <summary>
        /// Initialise the underlying Watchdog for C#.
        /// </summary>
        public override void Init()
        {
            base.Init();
            
            statementDelimiter = char.Parse(";");
            startBlockDelimiter = char.Parse("{");
            endBlockDelimiter = char.Parse("}");
            stringDelimiters = new List<char>() {char.Parse("\"")};
            stringEscape = char.Parse("\\");
            startCommentDelimiter = "//";
            endCommentDelimiter = "\n";
            
            errorCodeStrings = new Dictionary<int, string>();
            
            errorCodeStrings[TabError] = "Tabs instead of spaces used for indentation";
            errorCodeStrings[PascalCaseError] = "Identifier is not in PascalCase";
            errorCodeStrings[CamelCaseError] = "Identifier is not in camelCase";
            errorCodeStrings[SpecialCharacterError] = "Disallowed character used in identifier";
            errorCodeStrings[MissingBracesError] = "Missing curly braces in if / while / foreach / for";
            errorCodeStrings[MultipleStatementError] = "Multiple statements on a single line";
            errorCodeStrings[CommentOnSameLineError] = "Comment not on a separate line";
            errorCodeStrings[CommentNoSpaceError] = "No space between comment delimiter and comment text";
            errorCodeStrings[ClassNotDocumentedError] = "Public class not documented";
            errorCodeStrings[MethodNotDocumentedError] = "Public method not documented";
            errorCodeStrings[InterfaceNamingError] = "Interface name does not begin with an I";
            
            statementHandler += CheckStatement;
            commentHandler += CheckComment;
            startBlockHandler += CheckStartBlock;
        }
        
        /// <summary>
        /// Check a single statement.
        /// </summary>
        /// <param name="statement">A string containing a statement, possibly multi-line.</param>
        void CheckStatement(string statement)
        {
            // TabError
            //
            if (statement.Contains("\t"))
            {
                if (errorCodeCount.ContainsKey(TabError))
                {
                    errorCodeCount[TabError] += 1;
                }
                else
                {
                    errorCodeCount[TabError] = 1;
                }
                
                // TODO: The line report is inaccurate, as several lines may have passed.
                // HACK: Assuming the next line and using CheckedLinesOfCode + 1.
                //
                if (woff != null)
                {
                    woff(string.Format("{0} (line {1})", errorCodeStrings[TabError], checkedLinesThisFile + 1));
                }
            }
            
            // MultipleStatementError
            //
            // Trim leading spaces before check.
            // Ignore empty statements, e.g. inline 'new' statements.
            // Ignore comparison operators, as they most probably are part of a 'for' loop.
            // Ignore single closing braces, mosrt probably closing inline lambdas.
            // Ignore 'get' and 'set': Properties are OK in a single line.
            //
            if (checkedLinesThisFile > 1
                && statement.Length > 0
                && !statement.TrimStart(char.Parse(" "), char.Parse("\r"), char.Parse("\t")).StartsWith("\n")
                && !statement.Contains("<")
                && !statement.Contains(">")
                && statement != ")"
                && !statement.Contains("get")
                && !statement.Contains("set"))
            {
                if (errorCodeCount.ContainsKey(MultipleStatementError))
                {
                    errorCodeCount[MultipleStatementError] += 1;
                }
                else
                {
                    errorCodeCount[MultipleStatementError] = 1;
                }
                
                if (woff != null)
                {
                    woff(string.Format("{0} (line {1})", errorCodeStrings[MultipleStatementError], checkedLinesThisFile + 1));
                }
            }
            
            // Identifiers
            //
            string possibleIdentifier = "";
            
            Match firstMatch = Regex.Match(statement,
                                           @"\s+\w+(<[\w, ]+>)?\s+(\w+)\s*$");
           
            // Ignore "as" casts.
            //
            if (firstMatch.Success && !statement.Contains(" as "))
            {
                possibleIdentifier = firstMatch.Groups[2].Value;
                
                Logging.Debug("Possible identifier: " + possibleIdentifier);
            }
            else
            {
                Match secondMatch = Regex.Match(statement,
                                                @"\s+\w+(<[\w, ]+>)?\s+(\w+)\s*=");
                
                if (secondMatch.Success)
                {
                    possibleIdentifier = secondMatch.Groups[2].Value;
                    
                    Logging.Debug("Possible identifier: " + possibleIdentifier);
                }
            }
            
            if (possibleIdentifier != ""
                && possibleIdentifier != "if"
                && possibleIdentifier != "else"
                && possibleIdentifier != "while"
                && possibleIdentifier != "foreach"
                && possibleIdentifier != "for"
                && !statement.Contains("using")
                && possibleIdentifier != "get"
                && possibleIdentifier != "set"
                && possibleIdentifier != "try"
                && possibleIdentifier != "catch"
                && possibleIdentifier != "delegate"
                && possibleIdentifier != "public"
                && possibleIdentifier != "switch")
            {
                // SpecialCharacterError
                //
                if (possibleIdentifier.Contains("_"))
                {
                    if (errorCodeCount.ContainsKey(SpecialCharacterError))
                    {
                        errorCodeCount[SpecialCharacterError] += 1;
                    }
                    else
                    {
                        errorCodeCount[SpecialCharacterError] = 1;
                    }
                    
                    // TODO: The line report is inaccurate, as several lines may have passed.
                    // HACK: Assuming the next line and using CheckedLinesOfCode + 1.
                    //
                    if (woff != null)
                    {
                        woff(string.Format("{0}: '{1}' (line {2})", errorCodeStrings[SpecialCharacterError], possibleIdentifier, checkedLinesThisFile + 1));
                    }
                }
                else
                {
                    if (statement.Contains("const "))
                    {
                        // PascalCaseError
                        //
                        if (possibleIdentifier.Length > 2 && char.IsLower(possibleIdentifier, 0))
                        {
                            if (errorCodeCount.ContainsKey(PascalCaseError))
                            {
                                errorCodeCount[PascalCaseError] += 1;
                            }
                            else
                            {
                                errorCodeCount[PascalCaseError] = 1;
                            }
                            
                            // TODO: The line report is inaccurate, as several lines may have passed.
                            // HACK: Assuming the next line and using CheckedLinesOfCode + 1.
                            //
                            if (woff != null)
                            {
                                woff(string.Format("{0}: '{1}' (line {2})", errorCodeStrings[PascalCaseError], possibleIdentifier, checkedLinesThisFile + 1));
                            }
                        }
                    }
                    else
                    {
                        // CamelCaseError
                        //
                        if (possibleIdentifier.Length > 2 && char.IsUpper(possibleIdentifier, 0))
                        {
                            if (errorCodeCount.ContainsKey(CamelCaseError))
                            {
                                errorCodeCount[CamelCaseError] += 1;
                            }
                            else
                            {
                                errorCodeCount[CamelCaseError] = 1;
                            }
                            
                            // TODO: The line report is inaccurate, as several lines may have passed.
                            // HACK: Assuming the next line and using CheckedLinesOfCode + 1.
                            //
                            if (woff != null)
                            {
                                woff(string.Format("{0}: '{1}' (line {2})", errorCodeStrings[CamelCaseError], possibleIdentifier, checkedLinesThisFile + 1));
                            }
                        }
                    }
                }
            }
            
            // MissingBracesError
            // Check for closing brace, indicating the statement is complete.
            //
            if ((statement.Trim().StartsWith("if")
                || statement.Trim().StartsWith("else")
                || statement.Trim().StartsWith("while")
                || statement.Trim().StartsWith("foreach")
                || statement.Trim().StartsWith("for"))
                && statement.Contains(")"))
            {
                if (errorCodeCount.ContainsKey(MissingBracesError))
                {
                    errorCodeCount[MissingBracesError] += 1;
                }
                else
                {
                    errorCodeCount[MissingBracesError] = 1;
                }
                
                // TODO: The line report is inaccurate, as several lines may have passed.
                // HACK: Assuming the next line and using CheckedLinesOfCode + 1.
                //
                if (woff != null)
                {
                    woff(string.Format("{0} (line {1})", errorCodeStrings[MissingBracesError], checkedLinesThisFile + 1));
                }
            }
            
            return;
        }
        
        /// <summary>
        /// Check a single comment.
        /// </summary>
        /// <param name="comment">A string containing a comment, possibly multi-line.</param>
        void CheckComment(string comment, string precedingInput)
        {
            // CommentOnSameLineError
            //
            if (checkedLinesThisFile > 1 && !precedingInput.Contains("\n"))
            {
                if (errorCodeCount.ContainsKey(CommentOnSameLineError))
                {
                    errorCodeCount[CommentOnSameLineError] += 1;
                }
                else
                {
                    errorCodeCount[CommentOnSameLineError] = 1;
                }
                
                if (woff != null)
                {
                    woff(string.Format("{0} (line {1})", errorCodeStrings[CommentOnSameLineError], checkedLinesThisFile));
                }
            }
            
            // CommentNoSpaceError
            // Also include /// doc comments.
            // Ignore empty comments.
            // Ignore comments starting with "--", these are most probably auto-generated decorative lines.
            //
            if (!comment.Trim().EndsWith(startCommentDelimiter)
                && !(comment.StartsWith(startCommentDelimiter + " ")
                     || comment.StartsWith(startCommentDelimiter + "/ "))
                && !comment.StartsWith(startCommentDelimiter + "--"))
            {
                if (errorCodeCount.ContainsKey(CommentNoSpaceError))
                {
                    errorCodeCount[CommentNoSpaceError] += 1;
                }
                else
                {
                    errorCodeCount[CommentNoSpaceError] = 1;
                }
                
                if (woff != null)
                {
                    woff(string.Format("{0} (line {1})", errorCodeStrings[CommentNoSpaceError], checkedLinesThisFile));
                }
            }
        }
        
        /// <summary>
        /// Checks the beginning of a block.
        /// </summary>
        /// <param name="startBlock">A string containing the start block.</param>
        void CheckStartBlock(string startBlock)
        {
            if (startBlock.Contains("class "))
            {
                // ClassNotDocumentedError
                //
                if (startBlock.Contains("public ")
                    && !previousToken.Contains(startCommentDelimiter + "/")
                    && !previousToken.Contains("</summary>"))
                {
                    if (errorCodeCount.ContainsKey(ClassNotDocumentedError))
                    {
                        errorCodeCount[ClassNotDocumentedError] += 1;
                    }
                    else
                    {
                        errorCodeCount[ClassNotDocumentedError] = 1;
                    }
                    
                    if (woff != null)
                    {
                        woff(string.Format("{0} (line {1})", errorCodeStrings[ClassNotDocumentedError], checkedLinesThisFile));
                    }
                }
                
                string className = "";
                
                Match classNameMatch = Regex.Match(startBlock, @"\Wclass\s+(\w+)");
                
                if (classNameMatch.Success)
                {
                    className = classNameMatch.Groups[1].Value;
                    
                    Logging.Debug("Class name: " + className);
                }
                
                // PascalCaseError
                //
                if (className.Length > 2
                    && char.IsLower(className, 0))
                {
                    if (errorCodeCount.ContainsKey(PascalCaseError))
                    {
                        errorCodeCount[PascalCaseError] += 1;
                    }
                    else
                    {
                        errorCodeCount[PascalCaseError] = 1;
                    }
                    
                    // TODO: The line report is inaccurate, as several lines may have passed.
                    // HACK: Assuming the next line and using CheckedLinesOfCode + 1.
                    //
                    if (woff != null)
                    {
                        woff(string.Format("{0}: '{1}' (line {2})", errorCodeStrings[PascalCaseError], className, checkedLinesThisFile));
                    }
                }
            }
            else if (startBlock.Contains("enum "))
            {
                string enumName = "";
                
                Match enumNameMatch = Regex.Match(startBlock, @"enum\s+(\w+)");
                
                if (enumNameMatch.Success)
                {
                    enumName = enumNameMatch.Groups[1].Value;
                    
                    Logging.Debug("Enum name: " + enumName);
                }
                
                // PascalCaseError
                //
                if (enumName.Length > 2 && char.IsLower(enumName, 0))
                {
                    if (errorCodeCount.ContainsKey(PascalCaseError))
                    {
                        errorCodeCount[PascalCaseError] += 1;
                    }
                    else
                    {
                        errorCodeCount[PascalCaseError] = 1;
                    }
                    
                    // TODO: The line report is inaccurate, as several lines may have passed.
                    // HACK: Assuming the next line and using CheckedLinesOfCode + 1.
                    //
                    if (woff != null)
                    {
                        woff(string.Format("{0}: '{1}' (line {2})", errorCodeStrings[PascalCaseError], enumName, checkedLinesThisFile));
                    }
                }
            }
            else if (startBlock.Contains("interface "))
            {
                string interfaceName = "";
                
                Match interfaceNameMatch = Regex.Match(startBlock, @"interface\s+(\w+)");
                
                if (interfaceNameMatch.Success)
                {
                    interfaceName = interfaceNameMatch.Groups[1].Value;
                    
                    Logging.Debug("Interface name: " + interfaceName);
                }
                
                // InterfaceNamingError
                //
                if (interfaceName.Length > 2 && !interfaceName.StartsWith("I"))
                {
                    if (errorCodeCount.ContainsKey(InterfaceNamingError))
                    {
                        errorCodeCount[InterfaceNamingError] += 1;
                    }
                    else
                    {
                        errorCodeCount[InterfaceNamingError] = 1;
                    }
                    
                    // TODO: The line report is inaccurate, as several lines may have passed.
                    // HACK: Assuming the next line and using CheckedLinesOfCode + 1.
                    //
                    if (woff != null)
                    {
                        woff(string.Format("{0}: '{1}' (line {2})", errorCodeStrings[InterfaceNamingError], interfaceName, checkedLinesThisFile));
                    }
                }
            }
            else if (startBlock.Contains("(") && startBlock.Contains(")"))
            {
                // MethodNotDocumentedError
                //
                // Make sure 'public' is before the first brace.
                //
                if (startBlock.Split(Char.Parse("("))[0].Contains("public ")
                    && !previousToken.Contains(startCommentDelimiter + "/")
                    && !previousToken.Contains("</summary>"))
                {
                    if (errorCodeCount.ContainsKey(MethodNotDocumentedError))
                    {
                        errorCodeCount[MethodNotDocumentedError] += 1;
                    }
                    else
                    {
                        errorCodeCount[MethodNotDocumentedError] = 1;
                    }
                    
                    if (woff != null)
                    {
                        woff(string.Format("{0} (line {1})", errorCodeStrings[MethodNotDocumentedError], checkedLinesThisFile));
                    }
                }
                
                string methodName = "";
                
                Match methodNameMatch = Regex.Match(startBlock, @"\w+\s+(\w+)\s*\(");
                
                if (methodNameMatch.Success)
                {
                    methodName = methodNameMatch.Groups[1].Value;
                    
                    Logging.Debug("Method name: " + methodName);
                }
                
                // PascalCaseError
                //
                if (methodName.Length > 2
                    && char.IsLower(methodName, 0)
                    && methodName != "if"
                    && methodName != "else"
                    && methodName != "while"
                    && methodName != "foreach"
                    && methodName != "for"
                    && methodName != "get"
                    && methodName != "set"
                    && methodName != "try"
                    && methodName != "catch"
                    && methodName != "delegate"
                    && methodName != "using"
                    && methodName != "public"
                    && methodName != "switch")
                {
                    if (errorCodeCount.ContainsKey(PascalCaseError))
                    {
                        errorCodeCount[PascalCaseError] += 1;
                    }
                    else
                    {
                        errorCodeCount[PascalCaseError] = 1;
                    }
                    
                    // TODO: The line report is inaccurate, as several lines may have passed.
                    // HACK: Assuming the next line and using CheckedLinesOfCode + 1.
                    //
                    if (woff != null)
                    {
                        woff(string.Format("{0}: '{1}' (line {2})", errorCodeStrings[PascalCaseError], methodName, checkedLinesThisFile));
                    }
                }
            }
            else if (!startBlock.Contains("if")
                     && !startBlock.Contains("else")
                     && !startBlock.Contains("while")
                     && !startBlock.Contains("foreach")
                     && !startBlock.Contains("for")
                     && !startBlock.Contains("get")
                     && !startBlock.Contains("set")
                     && !startBlock.Contains("try")
                     && !startBlock.Contains("catch")
                     && !startBlock.Contains("delegate")
                     && !startBlock.Contains("using")
                     && !startBlock.Contains("public")
                     && !startBlock.Contains("switch"))
            {
                // Assuming it's a property
                
                string propertyName = "";
                
                Match propertyNameMatch = Regex.Match(startBlock, @"\s+(\w+)\s*$");
                
                if (propertyNameMatch.Success)
                {
                    propertyName = propertyNameMatch.Groups[1].Value;
                    
                    Logging.Debug("Property name: " + propertyName);
                }
                
                // PascalCaseError
                //
                if (propertyName.Length > 2 && char.IsLower(propertyName, 0))
                {
                    if (errorCodeCount.ContainsKey(PascalCaseError))
                    {
                        errorCodeCount[PascalCaseError] += 1;
                    }
                    else
                    {
                        errorCodeCount[PascalCaseError] = 1;
                    }
                    
                    // TODO: The line report is inaccurate, as several lines may have passed.
                    // HACK: Assuming the next line and using CheckedLinesOfCode + 1.
                    //
                    if (woff != null)
                    {
                        woff(string.Format("{0}: '{1}' (line {2})", errorCodeStrings[PascalCaseError], propertyName, checkedLinesThisFile));
                    }
                }
            }
            
            return;
        }
    }
}

