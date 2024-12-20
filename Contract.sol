// SPDX-License-Identifier: MIT
pragma solidity ^0.8.17;

contract BoxDashGameFi {
    mapping(address => uint256) private playerScores;

    event ScoreIncreased(address indexed player, uint256 newScore);

    // Function to get the player's current score
    function getPlayerScore(address player) external view returns (uint256) {
        return playerScores[player];
    }

    // Function to add 1 point to the player's score
    function addOnePoint(address player) external {
        require(player != address(0), "Invalid address");

        playerScores[player] += 1;

        emit ScoreIncreased(player, playerScores[player]);
    }
}
