using System.Text;

namespace Sholo.Mqtt.Topics.FilterSanitizer;

internal static class TopicFilterSanitizer
{
    public static string SanitizeTopic(string topicPattern)
    {
        var sb = new StringBuilder(topicPattern.Length);
        var parts = topicPattern.Split('/');

        foreach (var part in parts)
        {
            if (part.StartsWith('+'))
            {
                sb.Append('+');
            }
            else if (part.StartsWith('#'))
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
