using AccountLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataGrid_1.Repository.Dto;

public record TransactionDto(
    decimal Amount,
    DateTime Date,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    TransactionType Type
);
//property attribute pentru a specifica ca enum-ul TransactionType trebuie serializat/deserializat ca string in JSON
