USE [ServicioFacturacion]
GO
/****** Object:  StoredProcedure [dbo].[SP_Timbrado_Get_Configs]    Script Date: 21/03/2019 08:58:11 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Roberto Jiménez
-- Create date: 08/marzo/2019
-- Description:	<
--		Obtiene las configuraciones para el AddOn de Timbrado
-- >
-- =============================================
ALTER PROCEDURE [dbo].[SP_Timbrado_Get_Configs]
	-- Add the parameters for the stored procedure here
	@DBName VARCHAR(30) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT * FROM(
		SELECT IDSuc
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'ContratoProd' THEN valor ELSE '' END) ContratoProd
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'UsuarioProd' THEN valor ELSE '' END) UsuarioProd
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'PasswProd' THEN valor ELSE '' END) PasswProd
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'XmlTimbradoPath' THEN valor ELSE '' END) XmlTimbradoPath
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'ArchivoCertificado' THEN valor ELSE '' END) ArchivoCertificado
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'CertificadoSAT' THEN valor ELSE '' END) CertificadoSAT
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'KeySAT' THEN valor ELSE '' END) KeySAT
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'PasswCertificado' THEN valor ELSE '' END) PasswCertificado
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'SMTPServer' THEN valor ELSE '' END) SMTPServer
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'SMTPPort' THEN valor ELSE '' END) SMTPPort
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'SMTPSSL' THEN valor ELSE '' END) SMTPSSL
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'SMTPUser' THEN valor ELSE '' END) SMTPUser
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'SMTPPassword' THEN valor ELSE '' END) SMTPPassword
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'SENDerEmail' THEN valor ELSE '' END) SENDerEmail
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'CCEmails' THEN valor ELSE '' END) CCEmails
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'Body' THEN valor ELSE '' END) Body
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'SAPB1DB' THEN valor ELSE '' END) SAPB1DB
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'Mail' THEN valor ELSE '' END) Mail
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'RutaRepCrystal' THEN valor ELSE '' END) RutaRepCrystal
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'EnviaCorreoCan' THEN valor ELSE '' END) EnviaCorreoCan
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'EnviaCorreo' THEN valor ELSE '' END) EnviaCorreo
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'XmlCanceladoPath' THEN valor ELSE '' END) XmlCanceladoPath
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'Subject' THEN valor ELSE '' END) [Subject]
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'Server' THEN valor ELSE '' END) Server
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'NewDB' THEN valor ELSE '' END) NewDB
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'CompanyName' THEN valor ELSE '' END) CompanyName
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'DBName' THEN valor ELSE '' END) DBName
		-- Parametros agregados del archivo App.config
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'AmbientePruebaTimbrado' THEN valor ELSE '' END) AmbientePruebaTimbrado
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'PruebaCorreo' THEN valor ELSE '' END) PruebaCorreo
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'XsltCadenaOriginal' THEN valor ELSE '' END) XsltCadenaOriginal
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'XsltCadenaOriginalImpresa' THEN valor ELSE '' END) XsltCadenaOriginalImpresa
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'XsltCadenaOriginalPagos' THEN valor ELSE '' END) XsltCadenaOriginalPagos
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'RutaGeneradorPDF' THEN valor ELSE '' END) RutaGeneradorPDF
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'VersionCfdi' THEN valor ELSE '' END) VersionCfdi
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'TaxCodeRetenciones' THEN valor ELSE '' END) TaxCodeRetenciones
		, MAX(CASE WHEN LTRIM(RTRIM(parametro)) = 'UpdatesDirectos' THEN valor ELSE '' END) UpdatesDirectos
	FROM 
		Parametrizaciones
	GROUP BY
		IDSuc) T1
	WHERE T1.DBName = @DBName
	;
END
