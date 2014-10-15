using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Datos
{
    public static class Constantes
    {
        //        
        //Constantes para el manejo de grupo de usuarios
        //

        public const int CONST_EstadoFiduciaria = 3;

        #region Constantes Evaluador

        public const int ConstModeloFinanciero = 1;
        public const int ConstSubIndicadoresFinancieros = 2;
        public const int ConstSubModeloFinanciero = 3;
        public const int ConstTablaEvaluacion = 4;
        public const int ConstSubGenerales = 5;
        public const int ConstSubComerciales = 6;
        public const int ConstSubTecnicos = 7;
        public const int ConstSubOrganizacionales = 8;
        public const int ConstSubFinancieros = 9;
        public const int ConstSubMedioAmbiente = 10;
        public const int ConstInformeConsolidado = 11;
        public const int ConstSubAspectosEvaluados = 12;
        public const int ConstSubObservaciones = 13;
        public const int ConstSubConclusion = 14;
        public const int ConstIndicadoresGestion = 15;
        public const int ConstSubProductosIndicadores = 17;
        public const int ConstSubEvaluacionProyecto = 20;
        public const int ConstSubFlujoCaja = 22;
        public const int ConstSubRiesgosIdentificados = 23;
        public const int ConstSubCentralesRiesgo = 21;
        public const int ConstInformes = 24;
        public const int ConstDesempenoEvaluador = 19;
        public const int ConstPlanOperativoEval = 25;
        public const int ConstSubPlanOperativoEval = 26;
        public const int ConstSubNomina = 27;
        public const int SubProduccion = 28;
        public const int ConstSubVentas = 29;
        public const int ConstHojaAvance = 30;

        #endregion

        // constantes para el windows open -- propiedades

        public static string parametrosOpen = "menubar={0},scrollbars={1},width={2},height={3},top={4}";

        public const int CONST_GerenteAdministrador = 1;
        public const int CONST_AdministradorFonade = 2;
        public const int CONST_AdministradorSena = 3;
        public const int CONST_JefeUnidad = 4;
        public const int CONST_Asesor = 5;
        public const int CONST_Emprendedor = 6;
        public const int CONST_CallCenter = 8;
        public const int CONST_GerenteEvaluador = 9;
        public const int CONST_CoordinadorEvaluador = 10;
        public const int CONST_Evaluador = 11;
        public const int CONST_GerenteInterventor = 12;
        public const int CONST_CoordinadorInterventor = 13;
        public const int CONST_Interventor = 14;
        public const int CONST_Perfil_Fiduciario = 15;
        public const int CONST_EstadoCoordinador = 2;

        //
        //Constantes para el manejo de roles de los estado del proyecto
        //
        public const int CONST_Inscripcion = 1;
        public const int CONST_PlanAprobado = 2;
        public const int CONST_Convocatoria = 3;
        public const int CONST_Evaluacion = 4;
        public const int CONST_AsignacionRecursos = 5;
        public const int CONST_LegalizacionContrato = 6;
        public const int CONST_Ejecucion = 7;
        public const int CONST_EvaluacionIndicadores = 8;
        public const int CONST_Condonacion = 9;
        //
        //Constantes para el manejo de roles de usuario dentro del proyecto
        //
        public const int CONST_RolAsesorLider = 1;
        public const int CONST_RolAsesor = 2;
        public const int CONST_RolEmprendedor = 3;
        public const int CONST_RolEvaluador = 4;
        public const int CONST_RolCoordinadorEvaluador = 5;
        public const int CONST_RolInterventor = 6;
        public const int CONST_RolCoordinadorInterventor = 7;
        public const int CONST_RolInterventorLider = 8;
        public const int CONST_RolAcreditador = 9;
        //
        //Constante para identificar la Unidad Temporal
        //
        public const int CONST_UnidadTemporal = 2;
        //
        //Constantes de tipos de Tareas
        //
        public const int CONST_Reunion = 1;
        public const int CONST_Generica = 2;
        public const int CONST_AsignarAsesor = 3;
        public const int CONST_PostIt = 5;
        public const int CONST_AsignarEvaluador = 8;
        public const int CONST_AsignarCoordinador = 10;
        public const int CONST_AsignarInterventor = 11;
        public const int CONST_AsignarCoordinadorInterventoria = 12;

        //
        //Constantes TABS proyecto
        //
        public const int CONST_Mercado = 12;
        public const int CONST_Operacion = 13;
        public const int CONST_Organizacion = 14;
        public const int CONST_Finanzas = 15;
        public const int CONST_PlanOperativo = 16;
        public const int CONST_Impacto = 17;
        public const int CONST_ResumenEjecutivo = 18;
        public const int CONST_Anexos = 19;
        public const int CONST_InvestigacionMercados = 20;
        public const int CONST_EstrategiasMercado = 21;
        public const int CONST_ProyeccionesVentas = 22;
        public const int CONST_SubOperacion = 23;
        public const int CONST_CostosInsumos = 24;
        public const int CONST_EstrategiaOrganizacional = 25;
        public const int CONST_EstructuraOrganizacional = 26;
        public const int CONST_AspectosLegales = 27;
        public const int CONST_Presupuestos = 28;
        public const int CONST_SubResumenEjecutivo = 29;
        public const int CONST_EquipoTrabajo = 30;
        public const int CONST_Infraestructura = 31;
        public const int CONST_Compras = 32;
        public const int CONST_Ingresos = 33;
        public const int CONST_Egresos = 34;
        public const int CONST_CapitalTrabajo = 35;
        public const int CONST_SubPlanOperativo = 36;
        public const int CONST_SubMetas = 37;

        public const int CONST_Empresa = 38;	//modificar a 38

        public const int CONST_RegistroMercantilInter = 39;		//Se debe eliminar
        public const int CONST_AvanceInter = 40;				//Se debe eliminar
        public const int CONST_IndicadoresGestionInter = 41;	//Se debe eliminar

        //
        //Constantes TABS Interventoria
        //
        public const int CONST_PlanOperativoInter = 1;
        public const int CONST_IndicadoresGestionInter2 = 2;
        public const int CONST_RiesgosInter = 3;
        public const int CONST_ConceptosInter = 4;
        public const int CONST_ContratoInter = 5;
        public const int CONST_PlanOperativoInter2 = 6;
        public const int CONST_NominaInter = 7;
        public const int CONST_ProduccionInter = 8;
        public const int CONST_VentasInter = 9;
        public const int CONST_IndicadoresGen = 10;
        public const int CONST_IndicadoresEsp = 11;

        //
        //Constantes para el manejo de los periodos
        //
        public const int CONST_Mes = 1;
        public const int CONST_Bimestre = 2;
        public const int CONST_Trimestre = 3;
        public const int CONST_Semestre = 4;

        //
        //Constantes para el manejo de Listado de Tipo Inversion  para Nuevas Inversiones (Pestana Finanzaz - Egresos), DropDownlList
        //
        public const string CONST_FondoEmprender = "Fondo Emprender";
        public const string CONST_AporteEmprendedores = "Aporte Emprendedores";
        public const string CONST_RecursosCapital = "Recursos de Capital";
        public const string CONST_IngresosVentas = "Ingresos por ventas";

        //
        //Constantes para el manejo de Listado de Tipo Aporte para Nuevos Aportes (Pestana Finanzaz - Ingreso), DropDownlList
        //
        public const string CONST_Dinero = "Dinero";
        public const string CONST_Bien = "Bien";
        public const string CONST_Servicio = "Servicio";


        //
        //Constantes para el manejo de Listado de Tipo Recruso para Nuevos Recurso (Pestana Finanzaz - Ingreso), DropDownlList
        //
        public const string CONST_Credito = "Crédito";
        public const string CONST_Donancion = "Donación";

        //Constantes para el manejo de evaluador
        public const int CONST_subAportes = 16;



        //Constantes de Acreditacion de Proyectos
        public const int CONST_Registro_y_Asesoria = 1;
        public const int CONST_Asignado_para_acreditacion = 10;
        public const int CONST_Aprobacion_Acreditacion = 11;
        public const int CONST_Aprobacion_No_Acreditacion = 12;
        public const int CONST_Acreditado = 13;
        public const int CONST_No_acreditado = 14;
        public const int CONST_Pendiente = 15;
        public const int CONST_Subsanado = 16;
        public const int CONST_Sena = 1;

        //Constantes TABS Evaluación.
        public const int CONST_PlanOperativoEval = 25;
        public const int CONST_subPlanOperativoEval = 26;
        public const int CONST_subNomina = 27;
        public const int CONST_subProduccion = 28;
        public const int CONST_subVentas = 29;
        public const int CONST_HojaAvance = 30;


        public const string CONST_TextoObsNoAcreditado = "NO ACREDITADO:";
        public const string CONST_TextoObsAcreditado = "ACREDITADO:";

        ///pagos

        public const int CONST_EstadoInterventor = 1; //Añadido el 12/04/2014. "Encontrada en el archivo (Pagos.inc)".
        public const int CONST_TipoPagoNomina = 2; //Añadido el 12/04/2014. "Encontrada en el archivo (Pagos.inc)".
        public const int CONST_EstadoAprobadoFA = 4;
        public const int CONST_EstadoRechazadoFA = 5;
        // constantes interventoria


        //Constantes internas.
        ///Añadido el 15/04/2014. "Encontrada y usada en InterProduccionDer.asp" entre otras páginas para
        ///generar multiplicación de valores y cantidades, generando los resultados en los campos correspondientes.
        public const int CONST_Meses = 12;
        public const int CONST_Fuentes = 2;

        //Constantes TABS Interventoria

        //Mas tabs NO incluídos inicialmente. (añadidos el 03/07/2014).
        public const int CONST_subGenerales = 5;
        public const int CONST_subComerciales = 6;
        public const int CONST_subTecnicos = 7;
        public const int CONST_subOrganizacionales = 8;
        public const int CONST_subFinancieros = 9;
        public const int CONST_subMedioAmbiente = 10;


        //Constantes que no estaban incluídas, se incluyeron el 07/07/2014 desde el archivo "Pagos.inc".

        //Constantes para determinar el tipo de pago: Actividad o Nomina
        public const int CONST_TipoPagoActividad = 1;

        //Constantes para determinar el estado de un pago
        public const int CONST_EstadoEdicion = 0;

        //COMENTADAS EN EL CLÁSICO.
        //'Numero de fideicomiso entre Fonade y Fiduagraria
        //'CONST CONST_Fideicomiso = 31204

        //COMENTADAS EN EL CLÁSICO.
        //'Numero de fideicomiso entre Fonade y FiduBogota
        //'CONST CONST_Fideicomiso = 312711
        public const int CONST_Fideicomiso = 3116428;

        //Numero de Encargo Fiduciario para la convocatoria 1
        public const int CONST_EncargoFiduciario1 = 103806;

        //Grupo fiduciaria
        public const int CONST_PerfilFiduciaria = 15;
    }
}
