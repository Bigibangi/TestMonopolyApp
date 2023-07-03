using NUnit.Framework;
using NSubstitute;
using FluentAssertions;
using Model;

namespace TestMonopolyApp {

    [TestFixture]
    internal class MainTest {
        private readonly IBaseContainerFactory<Box> _boxFactory = new BoxFactory<Box>();
        private readonly IBaseContainerFactory<Pallete> _palleteFactory = new PalleteFactory<Pallete>();

        //Когда создаем паллет, проверить его корректный объём
        [TestCase(1f, 1f, 0.2f, 0.2f)]
        [TestCase(1f, 0.5f, 0.5f, 0.25f)]
        [Test]
        public void WhenCreatePallet_AndCheckVolume_ThenVolumeShouldBeValid(float a, float b, float c, float v) {
            // Arrange.
            var pallete = _palleteFactory.CreateContaner(a,b,c,30);
            // Act.

            // Assert.
            pallete.Volume.Should().Be(v);
        }

        //Когда создаем паллет, проверить его корректный вес

        [TestCase(40f)]
        [TestCase(30f)]
        [Test]
        public void WhenCreatePallet_AndCheckWeight_ThenWeightShouldBeValid(float w) {
            // Arrange.
            var pallet = _palleteFactory.CreateContaner(1f,1f,1f,w);
            // Act.

            // Assert.
            pallet.Weight.Should().Be(w);
        }

        //Когда создаем короб, проверить его корректный объём

        [TestCase(1f, 1f, 0.2f, 0.2f)]
        [TestCase(1f, 0.5f, 0.5f, 0.25f)]
        [Test]
        public void WhenCreateBox_AndCheckVolume_ThenVolumeShouldBeValid(float a, float b, float c, float v) {
            // Arrange.
            var box =  _boxFactory.CreateContaner(a,b,c,30, DateOnly.FromDateTime(DateTime.Now));
            // Act.

            // Assert.
            box.Volume.Should().Be(v);
        }

        //Когда создаем короб, проверить его корректный вес
        [TestCase(30f)]
        [TestCase(40f)]
        [Test]
        public void WhenCreateBox_AndCheckWeight_ThenWeightShouldBeValid(float w) {
            // Arrange.
            var box = _boxFactory.CreateContaner(1f,1f,1f,w, DateOnly.FromDateTime(DateTime.Now));
            // Act.

            // Assert.
            box.Weight.Should().Be(w);
        }

        /*Если в конструктор передать дату более раннюю,
         * чем текущая, она автоматически станет датой производста.
        А срок годности будет дата производста + 100 дней*/

        [Test]
        public void WhenDateIsPast_AndCreateBox_ThenDateMustBeValid() {
            // Arrange.
            var date = new DateOnly(2023, 3, 5);
            var expectedDate = date.AddDays(100);
            // Act.
            var box = _boxFactory.CreateContaner(1f, 1f, 1f, 1f, date);
            // Assert.
            box.ProdDate.Should().Be(date);
            box.SuitDate.Should().Be(expectedDate);
        }

        //Если добавить коробку на пустую паллету, объем изменится корректно
        [TestCase(1f, 1f, 0.2f, 1.2f)]
        [TestCase(1.5f, 1f, 0.2f, 1f)]
        [Test]
        public void WhenPalleteIsEmpty_AndAddBox_ThenVolumeMustChangeValid(float a, float b, float c, float expectedVolume) {
            // Arrange.
            var pallete = _palleteFactory.CreateContaner(1f, 1f, 1f);
            var box = _boxFactory.CreateContaner(a, b, c, 30,DateOnly.FromDateTime(DateTime.Now));
            // Act.
            pallete.TryAddContainer<Box>(box);
            // Assert.
            pallete.Volume.Should().Be(expectedVolume);
        }

        //Если добавить коробку на пустую паллету, вес изменится корректно
        //Если паллет не пустой, проверить корректный вес всех коробок + 30 кг
        [TestCase(30, 60)]
        [TestCase(45, 75)]
        [Test]
        public void WhenPalleteIsEmpty_AndAddBox_ThenWeightMustChandegValid(float w, float expectedWeight) {
            // Arrange.
            var pallete = _palleteFactory.CreateContaner(1f,1f,1f);
            var box = _boxFactory.CreateContaner(1f, 1f, 1f, w,DateOnly.FromDateTime(DateTime.Now));
            // Act.
            pallete.TryAddContainer<Box>(box);
            // Assert.
            pallete.Weight.Should().Be(expectedWeight);
        }

        //Проверить корректную дату срока годности паллета (должна быть самая менее годная)

        [Test]
        public void WhenCreatePallete_AndPalleteNotEmpty_ThenSuitDateMustBeValid() {
            // Arrange.
            var pallete = _palleteFactory.CreateContaner(1f,1f,1f);
            var date = new DateOnly(2023, 6, 10);
            var box = _boxFactory.CreateContaner(1f,1f,1f,date: date);
            // Act.
            var expectedDate = date.AddDays(100);
            pallete.TryAddContainer<Box>(box);
            // Assert.

            pallete.SuitDate.Should().Be(expectedDate);
        }
    }
}