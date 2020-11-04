using Remotion.Linq.Clauses;
using RoomBooking.Core.Contracts;
using RoomBooking.Core.Entities;
using RoomBooking.Persistence;
using RoomBooking.Wpf.Common;
using RoomBooking.Wpf.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace RoomBooking.Wpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private ObservableCollection<Room> _rooms;
        private ObservableCollection<Booking> _bookings;
        private Booking _currentBooking;
        private Room _selectedRoom;

        public Booking CurrentBooking
        {
            get => _currentBooking;

            set
            {
                _currentBooking = value;
                OnPropertyChanged(nameof(CurrentBooking));
            }
        }
        public ObservableCollection<Room> Rooms
        {
            get => _rooms;
            set
            {
                _rooms = value;
                OnPropertyChanged(nameof(Rooms));
            }
        }
        public ObservableCollection<Booking> Bookings
        {
            get => _bookings;
            set
            {
                _bookings = value;
                OnPropertyChanged(nameof(Bookings));
            }
        }
        public Room SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                _selectedRoom = value;
                OnPropertyChanged(nameof(SelectedRoom));
                LoadDataAsync();
            }
        }
        public MainViewModel(IWindowController windowController) : base(windowController)
        {
        }

        private async Task LoadDataAsync()
        {
            using IUnitOfWork uow = new UnitOfWork();

            if (Rooms == null)
            {
                var rooms = (await uow.Rooms
                    .GetAllAsync())
                    .OrderBy(r => r.RoomNumber)
                    .ToList();

                Rooms = new ObservableCollection<Room>(rooms);
                SelectedRoom = Rooms.First();
            }

            var bookings = (await uow.Bookings
                .GetByRoomWithCustomerAsync(SelectedRoom.Id))
                .OrderBy(c => c.From)
                .ThenBy(c => c.To)
                .ToList();

            Bookings = new ObservableCollection<Booking>(bookings);
        }
        public static async Task<MainViewModel> CreateAsync(IWindowController windowController)
        {
            var viewModel = new MainViewModel(windowController);
            await viewModel.LoadDataAsync();
            return viewModel;
        }
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }

        private ICommand _cmdEditCustomerCommand;
        public ICommand CmdEditCustomerCommand 
        { 
            get
            {
                if(_cmdEditCustomerCommand == null)
                {
                    _cmdEditCustomerCommand = new RelayCommand(
                        execute: _ => Controller.ShowWindow(new EditCustomerViewModel(Controller, CurrentBooking.Customer), true),
                        canExecute: _ => CurrentBooking != null
                        );
                }
                return _cmdEditCustomerCommand;
            }
        }
    }
}
