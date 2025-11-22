using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerInsights.EmailService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fluid;

    public sealed class LiquidTemplateRenderer
    {
        private readonly FluidParser _parser;
        private readonly ILogger<LiquidTemplateRenderer> _logger;

        public LiquidTemplateRenderer(ILogger<LiquidTemplateRenderer> logger)
        {
            _logger = logger;
            _parser = new FluidParser();
        }

        public async Task<string?> RenderAsync(string template, IDictionary<string, object> values)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                _logger.LogError("Temolate is empty");
                return null;
            }

            if (_parser.TryParse(template, out IFluidTemplate fluidTemplate, out string errors) == false)
            {
                _logger.LogError("Liquid-Template ist ungültig: {Errors}", errors);
                return null;
            }

            TemplateContext context = new TemplateContext();

            if (values != null)
            {
                foreach (KeyValuePair<string, object> kvp in values)
                {
                    context.SetValue(kvp.Key, kvp.Value);
                }
            }

            string result = await fluidTemplate.RenderAsync(context);
            return result;
        }
    }

}
