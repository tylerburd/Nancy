using Nancy.Security;

namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Diagnostics;

    /// <summary>
    /// Nancy context.
    /// </summary>
    public sealed class NancyContext : IDisposable
    {
        private Request request;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyContext"/> class.
        /// </summary>
        public NancyContext()
        {
            this.Items = new Dictionary<string, object>();
            this.Trace = new RequestTrace();
            this.ViewBag = new DynamicDictionary();
            
            // TODO - potentially additional logic to lock to ip etc?
            this.ControlPanelEnabled = true;
        }

        /// <summary>
        /// Gets the dictionary for storage of per-request items. Disposable items will be disposed when the context is.
        /// </summary>
        public IDictionary<string, object> Items { get; private set; }

        /// <summary>
        /// Gets or sets the parameters for the resolved route 
        /// </summary>
        public dynamic Parameters { get; set; }

        /// <summary>
        /// Gets or sets the incoming request
        /// </summary>
        public Request Request
        {
            get
            {
                return this.request;
            }

            set
            {
                this.request = value;
                this.Trace.Method = request.Method;
                this.Trace.RequestUrl = request.Url;
            }
        }

        public INancyEngine NancyEngine { get; set; }

        /// <summary>
        /// Gets or sets the outgoing response
        /// </summary>
        public Response Response { get; set; }

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public IUserIdentity CurrentUser { get; set; }

        /// <summary>
        /// Diagnostic request tracing
        /// </summary>
        public RequestTrace Trace { get; private set; }

        /// <summary>
        /// Gets a value indicating whether control panel access is enabled for this request
        /// </summary>
        public bool ControlPanelEnabled { get; private set; }

        /// <summary>
        /// Non-model specific data for rendering in the response
        /// </summary>
        public dynamic ViewBag { get; private set; }

        /// <summary>
        /// Disposes any disposable items in the <see cref="Items"/> dictionary.
        /// </summary>
        public void Dispose()
        {
            foreach (var disposableItem in this.Items.Values.OfType<IDisposable>())
            {
                disposableItem.Dispose();
            }

            this.Items.Clear();

            if (this.request != null)
            {
                ((IDisposable) this.request).Dispose();
            }
        }
    }
}
