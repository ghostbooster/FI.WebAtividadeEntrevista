CREATE PROC FI_SP_ConsBeneficiariosCliente
	@IDCLIENTE BIGINT
AS
BEGIN
	SELECT ID, CPF, NOME FROM BENEFICIARIOS WITH(NOLOCK) WHERE IDCLIENTE = @IDCLIENTE 
END