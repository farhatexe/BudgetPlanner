using DataTables.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace BudgetPlanner.Models
{
    // Create a custom type from DataTablesBinder.
    public class TransactionSearchDataTablesBinder : DataTablesBinder
    {
        // Override the default BindModel called by ASP.NET and make it call Bind passing the type of your
        // implementation of IDataTablesRequest:
        public override object BindModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            return Bind(controllerContext, bindingContext, typeof(TransactionSearchRequest));
        }

        // Override MapAditionalProperties so you can set your aditional data into the model:
        protected override void MapAditionalProperties(IDataTablesRequest requestModel, NameValueCollection requestParameters)
        {
            var myModel = (TransactionSearchRequest)requestModel;
            myModel.eDate = Get<DateTime>(requestParameters, "eDate");
            myModel.sDate = Get<DateTime>(requestParameters, "sDate");
            myModel.sAmt = Get<Decimal>(requestParameters, "sAmt");
            myModel.eAmt = Get<Decimal>(requestParameters, "eAmt");
        }
    }

    // You'll need a custom request model, of course.
    // Just derive from DefaultDataTablesRequest and you're fine :)
    // You can choose to implement IDataTablesRequest too, if you like.
    public class TransactionSearchRequest : DefaultDataTablesRequest
    {
        public DateTimeOffset? sDate { get; set; }
        public DateTimeOffset? eDate { get; set; }
        public decimal? sAmt { get; set; }
        public decimal? eAmt { get; set; }
    }

}