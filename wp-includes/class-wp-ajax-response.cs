using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.Ajax
{
    public class WPAjaxResponse
    {
        /// <summary>
        /// Store XML responses to send.
        /// </summary>
        public List<XElement> Responses { get; set; } = new List<XElement>();

        private readonly AppDbContext _dbContext;

        public WPAjaxResponse(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Constructor - Passes args to WPAjaxResponse::Add().
        /// </summary>
        /// <param name="args">Optional. Will be passed to Add() method.</param>
        public WPAjaxResponse(Dictionary<string, object> args = null, AppDbContext dbContext = null)
        {
            _dbContext = dbContext ?? new AppDbContext();
            if (args != null && args.Count > 0)
            {
                Add(args);
            }
        }

        /// <summary>
        /// Appends data to an XML response based on given arguments.
        /// </summary>
        /// <param name="args">An array or dictionary of XML response arguments.</param>
        /// <returns>XML response as a string.</returns>
        public string Add(Dictionary<string, object> args)
        {
            var defaults = new Dictionary<string, object>
            {
                { "what", "object" },
                { "action", false },
                { "id", 0 },
                { "old_id", false },
                { "position", 1 },
                { "data", "" },
                { "supplemental", new Dictionary<string, string>() }
            };

            foreach (var key in defaults.Keys)
            {
                if (!args.ContainsKey(key))
                {
                    args[key] = defaults[key];
                }
            }

            var position = args["position"].ToString().Replace("[^a-z0-9:_-]", "");
            var id = args["id"].ToString();
            var what = args["what"].ToString();
            var action = args["action"] as bool? == false ? HttpContext.Current.Request.Form["action"] : args["action"].ToString();
            var oldId = args["old_id"];
            var data = args["data"];
            var supplemental = args["supplemental"] as Dictionary<string, string>;

            var responseXml = new StringBuilder();

            if (data is Exception error)
            {
                foreach (var errorCode in GetErrorCodes(error))
                {
                    responseXml.Append($"<wp_error code='{errorCode}'><![CDATA[{GetErrorMessage(error, errorCode)}]]></wp_error>");
                    var errorData = GetErrorData(error, errorCode);
                    if (errorData != null)
                    {
                        responseXml.Append($"<wp_error_data code='{errorCode}'>{errorData}</wp_error_data>");
                    }
                }
            }
            else
            {
                responseXml.Append($"<response_data><![CDATA[{data}]]></response_data>");
            }

            var supplementalXml = "";
            if (supplemental != null && supplemental.Any())
            {
                supplementalXml = "<supplemental>" + string.Join("", supplemental.Select(kvp => $"<{kvp.Key}><![CDATA[{kvp.Value}]]></{kvp.Key}>")) + "</supplemental>";
            }

            var xml = new XElement("response",
                new XAttribute("action", $"{action}_{id}"),
                new XElement(what,
                    new XAttribute("id", id),
                    oldId != false ? new XAttribute("old_id", oldId) : null,
                    new XAttribute("position", position),
                    XElement.Parse(responseXml.ToString()),
                    !string.IsNullOrEmpty(supplementalXml) ? XElement.Parse(supplementalXml) : null
                )
            );

            Responses.Add(xml);
            return xml.ToString();
        }

        /// <summary>
        /// Display XML formatted responses.
        /// </summary>
        public void Send()
        {
            HttpContext.Current.Response.ContentType = "text/xml; charset=utf-8";
            HttpContext.Current.Response.Write("<?xml version='1.0' encoding='utf-8' standalone='yes'?><wp_ajax>");
            foreach (var response in Responses)
            {
                HttpContext.Current.Response.Write(response.ToString());
            }
            HttpContext.Current.Response.Write("</wp_ajax>");
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Gets error codes from an exception.
        /// </summary>
        private IEnumerable<string> GetErrorCodes(Exception error)
        {
            // Simulate WP_Error behavior
            return new[] { "error_code" };
        }

        /// <summary>
        /// Gets error message for a specific code.
        /// </summary>
        private string GetErrorMessage(Exception error, string code)
        {
            return error.Message;
        }

        /// <summary>
        /// Gets error data for a specific code.
        /// </summary>
        private string GetErrorData(Exception error, string code)
        {
            return error.Data.Contains(code) ? error.Data[code].ToString() : null;
        }
    }
}