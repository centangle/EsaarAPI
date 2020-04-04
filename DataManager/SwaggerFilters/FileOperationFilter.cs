using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace DataManager.SwaggerFilters
{
    public class FileOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.operationId.ToLower() == "attachment_upload")
            {
                if (operation.parameters == null)
                    operation.parameters = new List<Parameter>(1);
                //else
                //    operation.parameters.Clear();
                operation.parameters.Add(new Parameter
                {
                    name = "File",
                    @in = "formData",
                    description = "Upload Attachment",
                    required = true,
                    type = "file"
                });
                operation.consumes.Add("application/form-data");
            }
        }
    }
}