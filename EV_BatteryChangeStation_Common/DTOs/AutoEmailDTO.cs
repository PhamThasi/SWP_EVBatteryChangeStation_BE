using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs
{
    public class AutoEmailDTO
    {
        public string RecipientEmail { get; set; }
        private static readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(500);
        private string _subject;
        private string _body;
        private string _footer;

        public string FontFamily { get; set; } = "Arial, sans-serif";
        public string FontSize { get; set; } = "14px";

        private string defaultFooter = $@"
        <div style='text-align: center; font-size: {{FontSize}}; color: #777; font-family: {{FontFamily}};'>
            <strong>Trân trọng,</strong><br>Đội ngũ hỗ trợ
        </div>";
        //======================

        public string Subject
        {
            get => ConvertToHtml(_subject);
            set => _subject = value;
        }

        private string ConvertToHtml(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            text = text.Replace("\n", "<br>").Replace(Environment.NewLine, "<br>");
            text = Regex.Replace(text, @"\|(.*?)\|", "<strong>$1</strong>", RegexOptions.Compiled | RegexOptions.IgnoreCase, Timeout);
            text = Regex.Replace(text, @"\[(.*?)\]", "<span style='color: gray;'>$1</span>", RegexOptions.Compiled | RegexOptions.IgnoreCase, Timeout);
            return text.Trim();
        }

        public string Body
        {
            get
            {
                return $@"
        <div style='font-family: {FontFamily}; font-size: {FontSize}; line-height: 1.6; color: #333; max-width: 600px; margin: auto; padding: 20px; 
                    border: 1px solid #ddd; border-radius: 10px; background-color: #fff;'>
            <h2 style='color: #007bff;'>Xin chào {RecipientEmail},</h2>
            <p>{ConvertToHtml(_body)}</p>
            <hr style='border-top: 1px solid #ddd; margin: 20px 0;'>{Footer}
        </div>";
            }
            set => _body = value;
        }

        public string Footer
        {
            get => string.IsNullOrEmpty(_footer) ? defaultFooter : WrapFooter(_footer);
            set => _footer = value;
        }

        private string WrapFooter(string customFooter)
        {
            return $@"
        <div style='text-align: center; font-size: {FontSize}; color: #777; font-family: {FontFamily};'>
            {ConvertToHtml(customFooter)}
        </div>";
        }

    }
    }

