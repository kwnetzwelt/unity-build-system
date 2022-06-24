using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UBS
{
    public class CommandLineArgsParser
    {
        private enum Context
        {
            None,
            Argument,
            Value,
        }

        public ArgsCollection Collection
        {
            get;
            private set;
        }
        
        public CommandLineArgsParser(string[] arguments)
        {
            /*arguments = new[]
            {
                "--myArg1:'value1'",
                "-myArg2:'value2'",
                "--myArg3:\"value3\"",
                "-myArg4:\"value4\"",
                "-myArg5:value5",
                "myArg6:value6",

                "--myArg7='value7'",
                "-myArg8='value8'",
                "--myArg9=\"value9\"",
                "-myArg10=\"value10\"",
                "-myArg11=value11",
                "myArg12=value12",

                "--myArg13 'value13'",
                "-myArg14 'value14'",
                "--myArg15 \"value15\"",
                "-myArg16 \"value16\"",
                "-myArg17 value17",
                "-myArg18", // no value
                
                "-myArg19 \"foo=bar\"", // "=" should be allowed as part of the value

                // "-" separated arg titles
                "-my-arg20",
                "--my-arg21",
                
            };
            */
            
            Collection = new ArgsCollection();
            Parse(string.Join(' ', arguments));
        }

        private static readonly char[] argSeparators = {'-'};
        private static readonly char[] valueSeparators = new[] {':', '=', ' '};
        private static readonly char[] quotes = new[] {'\"', '\'', '´', '`'};
        private static readonly char[] whitespace = new[] {' ', '\t'};
        void Parse(string arguments)
        {
            arguments = arguments.TrimEnd() + " ";
            int n;
            char c;
            char? parserContextInQuotes = null;
            Context parserContext = Context.None;
            bool wasArgSeparated  = false;
            bool isArgSeparator, isValueSeparator, isQuote, isWhitespace;
            StringBuilder builder = new StringBuilder();
            using (StringReader reader = new StringReader(arguments))
            {
                
                while ((n = reader.Read()) != -1)
                {
                    c = (char) n;

                    isQuote = Array.IndexOf(quotes, c) != -1;
                    if (isQuote)
                    {
                        if (parserContextInQuotes == c)
                        {
                            // if we are already within quotes, we can exit quotes
                            parserContextInQuotes = null;
                        }
                        else
                        {
                            // if we are not already within quotes, we activate it
                            parserContextInQuotes = c;
                            parserContext = Context.Value;
                        }
                        continue;
                    }

                    if (parserContextInQuotes == null) // ignore whitespaces in quotes
                    {

                        isWhitespace = Array.IndexOf(whitespace, c) != -1;
                        if (isWhitespace)
                        {
                            if (parserContext == Context.Argument)
                            {
                                // write argument name
                                Collection.Last.Name = builder.ToString();
                                builder.Clear();
                                parserContext = Context.None;
                                continue;
                            }

                            if (parserContext == Context.Value)
                            {
                                // write argument value
                                Collection.Last.Value = builder.ToString();
                                //Collection.Next();
                                builder.Clear();
                                wasArgSeparated = false;
                                parserContext = Context.None;
                                continue;
                            }
                        }
                        
                        isValueSeparator = Array.IndexOf(valueSeparators, c) != -1;
                        if (isValueSeparator)
                        {
                            if (parserContext == Context.Argument)
                            {
                                // write argument name
                                Collection.Last.Name = builder.ToString();
                                builder.Clear();
                                parserContext = Context.Value;
                                continue;
                            }
                            else
                                continue; // if we are not in argument context, we skip this char
                        }

                        isArgSeparator = Array.IndexOf(argSeparators, c) != -1;

                        if (!isWhitespace && !isValueSeparator && parserContext == Context.None)
                        {
                            if (wasArgSeparated && !isArgSeparator)
                            {
                                parserContext = Context.Value;
                            }
                            else if (isArgSeparator || !wasArgSeparated)
                            {
                                wasArgSeparated = isArgSeparator;
                                Collection.Next();
                                parserContext = Context.Argument;
                            }
                        }
                        
                        if(parserContext == Context.Argument && builder.Length == 0 && isArgSeparator)
                            continue; // skip "--" or even "----"
                    }

                    builder.Append(c);
                }
                
            }
            
            parserContext = Context.None;
            
            
            
        }

        public class Argument
        {
            public string Name { get; internal set; }
            public string Value { get; internal set; }

            public override string ToString()
            {
                return Name + " " + Value;
            }
        }

        public class ArgsCollection
        {
            
            public bool HasArgument(string argument)
            {
                return arguments.Find( (a) => a.Name == argument) != null;
            }

            public string GetValue<T>(string argument)
            {
                var found = arguments.Find((a) => a.Name == argument);
                if (found == null)
                    return null;
                return found.Value;
            }

            internal List<Argument> arguments = new List<Argument>();

            public List<Argument> Arguments => arguments;
            internal void Next()
            {
                arguments.Add(new Argument());
            }
            internal Argument Last
            {
                get
                {
                    if (arguments.Count == 0)
                    {
                        Next();
                    }

                    return arguments[^1];
                }
            }

            public int Count => arguments.Count;
        }
    }

}