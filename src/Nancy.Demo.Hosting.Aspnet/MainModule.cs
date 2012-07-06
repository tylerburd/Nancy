namespace Nancy.Demo.Hosting.Aspnet
{
    using System;
    using System.Linq;
    using Nancy.Demo.Hosting.Aspnet.Models;
    using Nancy.Routing;
    using Security;

    public class MainModule : NancyModule
    {
        public MainModule(IRouteCacheProvider routeCacheProvider)
        {
            Get["/"] = x => {
                return View["routes", routeCacheProvider.GetCache()];
            };

            Get["/filtered", r => true] = x => {
                return "This is a route with a filter that always returns true.";
            };

            Get["/filtered", r => false] = x => {
                return "This is also a route, but filtered out so should never be hit.";
            };

            Get[@"/(?<foo>\d{2,4})/{bar}"] = x => {
                return string.Format("foo: {0}<br/>bar: {1}", x.foo, x.bar);
            };

            Get["/test"] = x => {
                return "Test";
            };

            Get["/nustache"] = parameters => {
                return View["nustache", new { name = "Nancy", value = 1000000 }];
            };

            Get["/dotliquid"] = parameters => {
                return View["dot", new { name = "dot" }];
            };

            Get["/javascript"] = x => {
                return View["javascript.html"];
            };

            Get["/static"] = x => {
                return View["static"];
            };

            Get["/razor"] = x => {
                var model = new RatPack { FirstName = "Frank" };
                return View["razor.cshtml", model];
            };

            Get["/razorError"] = x =>
            {
                var model = new RatPack { FirstName = "Frank" };
                return View["razor-error.cshtml", model];
            };

            Get["/razor-simple"] = x =>
            {
                var model = new RatPack { FirstName = "Frank" };
                return View["razor-simple.cshtml", model];
            };

            Get["/razor-dynamic"] = x =>
            {
                return View["razor.cshtml", new { FirstName = "Frank" }];
            };

            Get["/razor-cs-strong"] = x =>
            {
                return View["razor-strong.cshtml", new RatPack { FirstName = "Frank" }];
            };

            Get["/razor-vb-strong"] = x =>
            {
                return View["razor-strong.vbhtml", new RatPack { FirstName = "Frank" }];
            };

            Get["/ssve"] = x =>
            {
                var model = new RatPack { FirstName = "You" };
                return View["ssve.sshtml", model];
            };

            Get["/viewmodelconvention"] = x => {
                return View[new SomeViewModel()];
            };

            Get["/viewwithchildview"] = x =>
            {
                var model = new RatPack { FirstName = "Parent" };
                return View["viewwithchildview.cshtml", model];
            };
            Get["/childview"] = x =>
            {
                var firstNameFromQueryString = Request.Query["FirstName"];
                var model = new RatPack { FirstName = firstNameFromQueryString };
                return View["childview.cshtml", model];
            };

            Get["/spark"] = x => {
                var model = new RatPack { FirstName = "Bright" };
                return View["spark.spark", model];
            };

            Get["/spark-anon"] = x =>
            {
                var model = new { FirstName = "Anonymous" };
                return View["anon.spark", model];
            };

            Get["/json"] = x => {
                var model = new RatPack { FirstName = "Andy" };
                return Response.AsJson(model);
            };

            Get["/xml"] = x => {
                var model = new RatPack { FirstName = "Andy" };
                return Response.AsXml(model);
            };

            Get["/session"] = x => {
                var value = Session["moo"] ?? "";

                var output = "Current session value is: " + value;

                if (String.IsNullOrEmpty(value.ToString()))
                {
                    Session["moo"] = "I've created a session!";
                }

                return output;
            };

            Get["/sessionObject"] = x => {
                var value = Session["baa"] ?? "null";

                var output = "Current session value is: " + value;

                if (value.ToString() == "null")
                {
                    Session["baa"] = new Payload(27, true, "some random string value");
                }

                return output;
            };

            Get["/error"] = x =>
                {
                    throw new NotSupportedException("This is an exception thrown in a route.");
                };

            Get["/customErrorHandler"] = _ => HttpStatusCode.ImATeapot;

            Get["/csrf"] = x => this.View["csrf", new { Blurb = "CSRF without an expiry using the 'session' token" }];

            Post["/csrf"] = x =>
            {
                this.ValidateCsrfToken();

                return string.Format("Hello {0}!", Request.Form.Name);
            };

            Get["/csrfWithExpiry"] = x =>
                {
                    // Create a new one because we have an expiry to check
                    this.CreateNewCsrfToken();

                    return this.View["csrf", new { Blurb = "You have 20 seconds to submit the page.. TICK TOCK :-)" }];
                };

            Post["/csrfWithExpiry"] = x =>
                {
                    this.ValidateCsrfToken(TimeSpan.FromSeconds(20));

                    return string.Format("Hello {0}!", Request.Form.Name);
                };

            Get["/viewNotFound"] = _ => View["I-do-not-exist"];

            Get["/fileupload"] = x =>
            {
                return View["FileUpload", new { Posted = "Nothing" }];
            };

            Post["/fileupload"] = x =>
            {
                var file = this.Request.Files.FirstOrDefault();

                string fileDetails = "Nothing";

                if (file != null)
                {
                    fileDetails = string.Format("{3} - {0} ({1}) {2}bytes", file.Name, file.ContentType, file.Value.Length, file.Key);
                }

                return View["FileUpload", new { Posted = fileDetails }];
            };

        }
    }
}
