CREATE PROC FI_SP_VerificaCliente
	@ID BIGINT,
	@CPF VARCHAR (11)
AS
BEGIN
	IF(ISNULL(@ID,0) > 0)
		SELECT 1 FROM CLIENTES WITH(NOLOCK) WHERE (CPF = @CPF) AND (ID <> @ID)
	ELSE
		SELECT 1 FROM CLIENTES WITH(NOLOCK) WHERE (CPF = @CPF)
END