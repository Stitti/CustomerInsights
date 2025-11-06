import {Flex} from "@radix-ui/themes";
import {MetricCard} from "./MetricCard";

export function MetricsTrends() {
    return (
        <Flex gap="4" direction="row" wrap="wrap">
            <MetricCard title="Satisfaction Index" value="78" trend={3}/>
            <MetricCard title="Interactions" value="1432" trend={-9}/>
            <MetricCard title="Model Confidence" value="82%" trend={4}/>
            <MetricCard title="High Urgency Interactions" value="212" trend={13}/>
        </Flex>
    )
}