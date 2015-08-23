(function (controllers, undefined) {

	/**
	 * @ngdoc controller
	 * @name 
	 * @function
	 * 
	 * @description
	 * The controller for the editing payment gateway settings
	 */
	controllers.QuickPayGatewayProviderController = function ($scope) {

		/**
		* @ngdoc method 
		* @name init
		* @function
		* 
		* @description
		* Method called on intial page load.  Loads in data from server and sets up scope.
		*/
		$scope.init = function () {

			//$scope.dialogData.provider.extendedData

			// on initial load extendedData will be empty but we need to populate with key values

			var settingsString = $scope.dialogData.provider.extendedData.items[0].value;
			$scope.quickpaySettings = JSON.parse(settingsString);

			// Watch with object equality to convert back to a string for the submit() call on the Save button
			$scope.$watch(function () {
				return $scope.quickpaySettings;
			}, function (newValue, oldValue) {
				$scope.dialogData.provider.extendedData.items[0].value = angular.toJson(newValue);
			}, true);
		};
		$scope.init();

	};

	angular.module("umbraco").controller("Merchello.Plugin.GatewayProviders.Payments.Dialogs.QuickPayGatewayProviderController", ['$scope', merchello.Controllers.QuickPayGatewayProviderController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));