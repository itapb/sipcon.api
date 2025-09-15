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
    public class Response<T>
    {
        
        public void SetNotFound()
        {

            Status = StatusCodes.Status200OK;
            Processed = false;
            Total = 0;
            Message = "REGISTROS NO ENCONTRADOS, POR FAVOR VALIDE LOS FILTROS APLICADOS!";
    

        }

        public void SetNotUpdated()
        {

            Status = StatusCodes.Status409Conflict;
            Processed = false;
            Total = 0;
            Message = "REGISTROS NO ACTUALIZADOS, POR FAVOR VALIDE LOS DATOS SELECCIONADOS!";
            //Data = new List<Models.Result>();
            //Data = new Models.Result();
        }

        public void SetOK()
        {

            Status = StatusCodes.Status200OK;
            Processed = true;
            Message = "CAMBIOS EFECTUADOS EXITOSAMENTE";

        }

        public void SetError(Exception ex)
        {
            Util.Log.Error(ex);
            Status = StatusCodes.Status409Conflict;
            Processed = false;
            Total = 0;
            // Solución: Asignar un valor compatible con T
            //if (typeof(T).IsGenericType && typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
            {
                //Type type = typeof(List<T>);
                //List<T> _list = (List<T>)Activator.CreateInstance(type);
                Data = (T)Activator.CreateInstance(typeof(T))!;


            }
            else
            {
                Type type = typeof(T);
                T _item = (T)Activator.CreateInstance(type);
                Data =_item;
            }
            Message = ex.Message;
        }

        public bool NotUpdated()
        {
            // Verifica si T es Models.Result antes de intentar convertir
            if (Data is Result result)
            {
                // Maneja el posible valor nulo de Data
                if (result.UpdatedRows == 0 && result.InsertedRows == 0)
                {
                    return true ;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                // Si T no es Models.Result, no se puede determinar si fue actualizado
                return false;
            }
        }

        public int Status { get; set; } = StatusCodes.Status200OK;
        public bool Processed { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public int Total { get; set; } = 0;
        public T Data { get; set; }

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

                //if (Data is IEnumerable)
                //{
                //    Data = new List<object>();
                //}
                //else
                //{
                //    Data = new object();
                //}

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
