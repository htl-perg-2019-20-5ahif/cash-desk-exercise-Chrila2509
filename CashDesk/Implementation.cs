using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CashDesk.Model;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CashDesk
{
    /// <inheritdoc />
    public class DataAccess : IDataAccess
    {
        private CashDeskContext context;

        private void IsContextValid()
        {
            if(context != null)
            {
                throw new InvalidOperationException();
            }
        }
        /// <inheritdoc />
        public Task InitializeDatabaseAsync()
        {
            IsContextValid();

            context = new CashDeskContext();
            // return context.SaveChanges();
            return Task.CompletedTask; // from your code
        }

        /// <inheritdoc />
        public async Task<int> AddMemberAsync(string firstName, string lastName, DateTime birthday)
        {
            IsContextValid();

            // check for duplicate not implemented yet
            var newMember = new Member
            {
                FirstName = firstName,
                LastName = lastName,
                Birthday = birthday
            };
            context.Members.Add(newMember);
            await context.SaveChangesAsync();
            return newMember.MemberNumber;
        }

        /// <inheritdoc />
        public async Task DeleteMemberAsync(int memberNumber)
        {
            IsContextValid();

            Member delMember;
            delMember = await context.Members.FirstOrDefaultAsync(m => m.MemberNumber == memberNumber);
            context.Members.Remove(delMember);
            await context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<IMembership> JoinMemberAsync(int memberNumber)
        {
            IsContextValid();
            // if(await context.Memberships.AnyAsync(m => m.Member.MemberNumber == memberNumber){
            //     if( check DateTime)
            // }
            // adopted all Memberships with DateTime check with your code (I forgot to check)
            if(await context.Memberships.AnyAsync(m => m.Member.MemberNumber == memberNumber && DateTime.Now >= m.Begin && DateTime.Now <= m.End))
            {
                throw new AlreadyMemberException();
            }

            var newMembership = new Membership
            {
                Member = await context.Members.FirstOrDefaultAsync(m => m.MemberNumber == memberNumber),
                Begin = DateTime.Now,
                End = DateTime.MaxValue
            };
            context.Memberships.Add(newMembership);
            return newMembership;
        }

        /// <inheritdoc />
        public async Task<IMembership> CancelMembershipAsync(int memberNumber)
        {
            IsContextValid();

            Membership cancMembership;
            
            if(await context.Memberships.AnyAsync(m => m.Member.MemberNumber == memberNumber && m.End >= DateTime.Now))
            {
                throw new NoMemberException();
            }
            cancMembership = await context.Memberships.FirstOrDefaultAsync(m => m.Member.MemberNumber == memberNumber);
            cancMembership.End = DateTime.Now;

            await context.SaveChangesAsync();

            return cancMembership;
        }

        /// <inheritdoc />
        public async Task DepositAsync(int memberNumber, decimal amount)
        {
            IsContextValid();

            if (await context.Members.AnyAsync(m => m.MemberNumber == memberNumber))
            {
                throw new ArgumentException();
            }
            if (await context.Memberships.AnyAsync(m => m.Member.MemberNumber == memberNumber && DateTime.Now >= m.Begin && DateTime.Now <= m.End))
            {
                throw new ArgumentException();
            }

            Deposit newDeposit = new Deposit
            {
                Membership = await context.Memberships.FirstOrDefaultAsync(m => m.Member.MemberNumber == memberNumber && DateTime.Now >= m.Begin && DateTime.Now <= m.End),
                Amount = amount
            };
            context.Deposits.Add(newDeposit);
            await context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IDepositStatistics>> GetDepositStatisticsAsync()
        {
            IsContextValid();

            var members = context.Members.Where(m => m.Memberships != null);
            var statistics = new List<DepositStatistics>();

            foreach(var member in members)
            {
                decimal amount = 0;
                int year = member.Memberships.Begin.Year;

                foreach (var d in member.Memberships.Deposits)
                {
                    amount += d.Amount;
                }
                statistics.Add(new DepositStatistics { Member = member, TotalAmount = amount, Year = year });
            }

            return statistics;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            IsContextValid();

            context.Dispose();
        }
    }
}
