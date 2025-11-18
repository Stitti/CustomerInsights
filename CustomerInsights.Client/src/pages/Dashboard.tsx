import {Box, Card, Flex, Text} from "@radix-ui/themes";
import {SentimentChart} from "../components/SentimentChart";
import {ChannelChart} from "../components/ChannelChart";
import TopAccountList from "../components/lists/TopAccountList";
import {MetricsTrends} from "../components/MetricsTrends";
import Header from "../components/headers/Header";
import React, { useState } from "react";
import type {TimeInterval} from "@/src/types.ts";

export function Dashboard() {
    const [interval, setInterval] = useState<TimeInterval>("0");
    return (
        <Box flexGrow="1" p="6">
            <Header
                showTimeInterval={true}
                showRefresh={true}
                selectedInterval={interval}
                onIntervalChange={setInterval}
            />

            <MetricsTrends timeInterval={interval} />

            <Flex direction="column" wrap="wrap">
                <Card style={{ flex: 1, minWidth: 320, zIndex: 4 }} variant="surface" size="3" mb="6">
                    <SentimentChart/>
                </Card>
                <Flex direction="row" gap="8" wrap="wrap">
                    <Card style={{ flex: 1, minWidth: 320 }} variant="surface" size="3" mb="6">
                        <Text size="5">Most used channels</Text>
                        <ChannelChart/>
                    </Card>
                    <Card style={{ flex: 1, minWidth: 320 }} variant="surface" size="3" mb="6">
                        <Text size="5">Most mentioned Categories</Text>
                        <ChannelChart/>
                    </Card>
                    <Card style={{ flex: 1, minWidth: 320 }} variant="surface" size="3" mb="6">
                        <Text size="5">Pain Points</Text>
                        <ChannelChart/>
                    </Card>
                </Flex>
                <Flex direction="row" gap="8" wrap="wrap">
                    <Card style={{ flex: 1 }} variant="surface" size="3" mb="6">
                        <Text size="5">Most satisfied customers</Text>
                        <TopAccountList/>
                    </Card>
                    <Card style={{ flex: 1 }} variant="surface" size="3" mb="6">
                        <Text size="5">Most dissatisfied customers</Text>
                        <TopAccountList/>
                    </Card>
                </Flex>
                <Flex direction="row" gap="8" wrap="wrap">
                    <Card style={{ flex: 1 }} variant="surface" size="3" mb="6">
                        <Text size="5">Top rated categories</Text>
                        <TopAccountList/>
                    </Card>
                    <Card style={{ flex: 1 }} variant="surface" size="3" mb="6">
                        <Text size="5">Worst rated categories</Text>
                        <TopAccountList/>
                    </Card>
                </Flex>
            </Flex>
        </Box>
    )
}