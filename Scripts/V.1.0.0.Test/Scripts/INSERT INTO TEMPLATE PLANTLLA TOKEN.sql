INSERT INTO TEMPLATE (VCONTENT,VDESCRIPTION,DCREATED)
VALUES( '<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8" />
  <title>Notificación de Seguridad — Sipcon</title>
</head>
<body style="margin:0; padding:0; background-color:#f3f4f6; font-family:Arial, sans-serif;">
  <table align="center" width="100%" cellpadding="0" cellspacing="0" style="padding:30px 0;">
    <tr>
      <td align="center">
        <table width="640" cellpadding="0" cellspacing="0" style="background-color:#ffffff; border:1px solid #d1d5db; border-radius:8px; box-shadow:0 4px 6px rgba(0,0,0,0.1);">
          
          <!-- Encabezado -->
          <tr>
            <td style="background-color:#1e3a8a; color:#ffffff; padding:24px;">
              <table width="100%">
                <tr>
                  <td width="50" valign="middle">
                    <img src="cid:logo_sipcon" alt="Logo institucional" style="width: 120px; height:auto;" />
                  </td>
                  <td valign="middle" style="padding-left:12px;">
                    <h1 style="margin:0; font-size:20px; font-weight:bold;">Sistema Sipcon</h1>
                    <p style="margin:0; font-size:14px; color:#c7d2fe;">Notificación de seguridad</p>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Cuerpo -->
          <tr>
            <td style="padding:32px; color:#1f2937; font-size:15px; line-height:1.6;">
              <p style="font-size:16px; margin-bottom:24px;">
                Estimado(a) <strong>{{user_name}}</strong>, se ha generado un código de verificación para validar su identidad en el sistema <strong>Sipcon</strong>.
              </p>

              <p style="font-weight:600; color:#374151; margin-bottom:8px;">Código de verificación:</p>
              <table width="100%" cellpadding="12" cellspacing="0" style="background-color:#fef3c7; border:1px solid #fde68a; border-radius:6px;">
                <tr>
                  <td style="color:#92400e; font-weight:600; font-size:18px; text-align:center;">
                    {{security_content}} <!-- Ejemplo: Código: <strong>ABC123</strong> -->
                  </td>
                </tr>
              </table>

              <p style="margin-top:16px; font-size:14px; color:#6b7280;">
                Este código es válido por tiempo limitado. No lo comparta con terceros. Si no ha solicitado este código, comuníquese con el administrador del sistema.
              </p>
            </td>
          </tr>

          <!-- Pie -->
          <tr>
            <td style="background-color:#f9fafb; padding:20px 32px; font-size:14px; color:#4b5563; border-top:1px solid #e5e7eb;">
              <table width="100%">
                <tr>
                  <td><strong style="color:#374151;">Remitente:</strong> {{sender}}</td>
                  <td align="right"><strong style="color:#374151;">Fecha:</strong> {{date_send}}</td>
                </tr>
              </table>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>','PLANTILLA DE SEGURIDAD CODIGO TEMPORAL',GETDATE())