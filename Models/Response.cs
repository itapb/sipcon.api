using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Util;

namespace Models
{
    public class Response
    {

        public void SetNotFound()
        {

            Status = StatusCodes.Status200OK;
            Processed = false;
            Total = 0;
            //Data = "{}";
            Message = "Registros no encontrados, por favor valide los filtros aplicados!...";

        }

        public void SetNotUpdated()
        {

            Status = StatusCodes.Status409Conflict;
            Processed = false;
            Total = 0;
            Message = "Registros no actualizados, por favor valide los datos selecionados!...";
            Data = new List<Models.Result>();

        }

        public void SetOK()
        {

            Status = StatusCodes.Status200OK;
            Processed = true;
            Message = "";

        }

        public void SetError(Exception ex)
        {
            Util.Log.Error(ex);
            Status = StatusCodes.Status409Conflict;
            Processed = false;
            Total = 0;
            Data = new List<Models.Result>();
            Message = ex.Message;

        }

        public bool NotUpdated()
        {

            if (((List<Result>)Data)[0].UpdatedRows == 0 && ((List<Result>)Data)[0].InsertedRows == 0 )
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        public int Status { get; set; } = StatusCodes.Status200OK;
        public bool Processed { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public int Total { get; set; } = 0;
        public object Data { get; set; } = new object();

        public void SetPostResponse()
        {

            if (NotUpdated())
            {
                SetNotUpdated();
            }
            else
            {
                SetOK();
            }


        }

        public void SetGetResponse(DataTable _dataTable, bool NotFoundIgnored = false )
        {

            var filas = _dataTable.Rows.Count;
            if (filas == 0)
            {
                if (!NotFoundIgnored){
                    SetNotFound();
                }

            }
            else
            {
                SetOK();
                try
                {
                    var valor = _dataTable.Rows[0]["TOTAL"];
                    Total = (int)valor;
                }
                catch
                {
                    Total = _dataTable.Rows.Count;
                }

            }


        }
    }
}
