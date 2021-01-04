using System;
using System.Text;

namespace Sholo.Mqtt.Topics.FilterSanitizer
{
    public static class TopicFilterSanitizer
    {
        public static string SanitizeTopic(string topicMask)
        {
            var sb = new StringBuilder(topicMask.Length);
            var parts = topicMask.Split('/');

            foreach (var part in parts)
            {
                if (part.StartsWith("+", StringComparison.Ordinal))
                {
                    sb.Append('+');
                }
                else if (part.StartsWith("#", StringComparison.Ordinal))
                {
                    sb.Append('#');
                }
                else
                {
                    sb.Append(part);
                }

                sb.Append('/');
            }

            if (sb.Length > 0)
            {
                sb.Length -= 1;
            }

            return sb.ToString();
        }
    }
}
