using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace TicketManagementSystem
{
    public class UserService : IDisposable
    {
        public User GetUser(string username, string failMessage)
        {
            User user = null;
            using (UserRepository userRepository = new UserRepository())
            {
                if (username != null)
                {
                    user = userRepository.GetUser(username);
                }
            }

            if (user == null)
            {
                throw new UnknownUserException(failMessage);
            }

            return user;

        }

        public User GetAccountManager()
        {

            using (UserRepository userRepository = new UserRepository())
            {
                return userRepository.GetAccountManager();
            }

        }

        public void Dispose()
        {

        }
    }
}
