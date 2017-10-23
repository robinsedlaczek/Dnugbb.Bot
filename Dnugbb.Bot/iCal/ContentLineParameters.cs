﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Dnugbb.Bot.iCal
{
    public class ContentLineParameters : Dictionary<string, ContentLineParameter>
    {
        private const string ParameterPattern = "([^;]+)(?=;|$)";

        public ContentLineParameters(string source)
        {
            MatchCollection matches = Regex.Matches(source, ParameterPattern);
            foreach (Match match in matches)
            {
                ContentLineParameter contentLineParameter = new ContentLineParameter(match.Groups[1].ToString());
                this[contentLineParameter.Name] = contentLineParameter;
            }
        }   

    }
}
