using System;
using System.Collections;
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
        private bool _listResponse = false;

        public Response(bool listResponse = false) {

            _listResponse=listResponse;
        }

        public void SetNotFound()
        {

            Status = StatusCodes.Status200OK;
            Processed = false;
            Total = 0;
            Message = "Registros no encontrados, por favor valide los filtros aplicados!...";
    

        }

        public void SetNotUpdated()
        {

            Status = StatusCodes.Status409Conflict;
            Processed = false;
            Total = 0;
            Message = "Registros no actualizados, por favor valide los datos selecionados!...";
            //Data = new List<Models.Result>();
            Data = new Models.Result();
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
            //Data = (_listResponse == true) ? new List<object>() : new object();
            Data = new Models.Result();
            Message = ex.Message;

        }

        public bool NotUpdated()
        {

            if (((Result)Data).UpdatedRows == 0 && ((Result)Data).InsertedRows == 0 )
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

        public void SetGetResponse(DataTable _dataTable )
        {

            var filas = _dataTable.Rows.Count;
            if (filas == 0)
            {
               
                SetNotFound();

                if (Data is IEnumerable)
                {
                    Data = new List<object>();
                }
                else
                {
                    Data = new object();
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
