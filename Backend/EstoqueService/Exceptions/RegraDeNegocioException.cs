namespace EstoqueService.Exceptions;

public class RegraDeNegocioException : Exception
{
    public RegraDeNegocioException(string message) : base(message) { }
}
