namespace Integrador.Mappers;
using Integrador.Models.Base;
using Enum = FindexMapper.Service.Enum;

public interface IBaseMapper<TInput, TResult>
    where TInput : class
    where TResult : class
{
    TResult Map(TInput? input, Enum.Environment environment);
}