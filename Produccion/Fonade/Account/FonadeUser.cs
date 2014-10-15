using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Web.Security;

namespace Fonade.Account
{
    public class FonadeUser : MembershipUser
    {
        private string nombres;

        public string Nombres
        {
            get { return nombres; }
            set { nombres = value; }
        }

        private string apellidos;

        public string Apellidos
        {
            get { return apellidos; }
            set { apellidos = value; }
        }

        private string Password;

        public string Password1
        {
            get { return Password; }
            set { Password = value; }
        }

        private int codGrupo;

        public int CodGrupo
        {
            get { return codGrupo; }
            set { codGrupo = value; }
        }

        private int idContacto;

        public int IdContacto
        {
            get { return idContacto; }
            set { idContacto = value; }
        }

        private int codInstitucion;

        public int CodInstitucion
        {
            get { return codInstitucion; }
            set { codInstitucion = value; }
        }

        private double identificacion;

        public double Identificacion
        {
            get { return identificacion; }
            set { identificacion = value; }
        }

        public FonadeUser(string providername,
            string username,
            object providerUserKey,
            string email,
            string passwordQuestion,
            string comment,
            bool isApproved,
            bool isLockedOut,
            DateTime creationDate,
            DateTime lastLoginDate,
            DateTime lastActivityDate,
            DateTime lastPasswordChangedDate,
            DateTime lastLockedOutDate,
            string nombres,
            string apellidos,
            int codGrupo,
            int idContacto,
            int codInstitucion,
            double identificacion,
            string Password1) :
            base(providername,
                username,
                providerUserKey,
                email,
                passwordQuestion,
                comment,
                isApproved,
                isLockedOut,
                creationDate,
                lastLoginDate,
                lastActivityDate,
                lastPasswordChangedDate,
                lastLockedOutDate)
        {
            this.nombres = nombres;
            this.apellidos = apellidos;
            this.codGrupo = codGrupo;
            this.idContacto = idContacto;
            this.codInstitucion = codInstitucion;
            this.identificacion = identificacion;
            this.Password = Password1;
        }

    }
}