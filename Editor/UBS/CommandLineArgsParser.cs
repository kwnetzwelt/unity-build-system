using System;
using System.IO;
using System.Text;
using UnityEditor;

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

        public CommandlineArgsCollection Collection
        {
            get;
            private set;
        }
        
        public CommandLineArgsParser(params string[] arguments)
        {
            
            
            Collection = new CommandlineArgsCollection();
            Parse(arguments);
        }

        private static readonly char[] argSeparators = {'-'};
        private static readonly char[] valueSeparators = new[] {':', '=', ' '};
        private static readonly char[] quotes = new[] {'\"', '\'', '´', '`'};
        private static readonly char[] whitespace = new[] {' ', '\t'};

        void Parse(string[] arguments)
        {
            
            foreach (var argument in arguments)
            {
                if( Array.IndexOf(argSeparators, argument[0]) != -1)
                    ParseAsArgument(argument);
                else
                    ParseAsValue(argument);
            }
        }

        void ParseAsArgument(string argumentSection)
        {
            Collection.Next();
            Collection.Last.Name = argumentSection.TrimStart(argSeparators);
        }

        void ParseAsValue(string argumentSection)
        {
            Collection.Last.Value = argumentSection;
        }
        
        void Parse(string arguments)
        {
            
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

            public Argument(Argument other)
            {
                Name = other.Name;
                Value = other.Value;
            }

            public Argument()
            {
                
            }
        }
    }
}