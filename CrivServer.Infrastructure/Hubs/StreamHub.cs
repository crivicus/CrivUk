﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CrivServer.Infrastructure.Hubs
{
    public class StreamHub : Hub
    {
        public ChannelReader<int> Counter(int count, int delay)
        {
            var channel = Channel.CreateUnbounded<int>();

            // We don't want to await WriteItems, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.
            _ = WriteItems(channel.Writer, count, delay);

            return channel.Reader;
        }

        private async Task WriteItems(ChannelWriter<int> writer, int count, int delay)
        {
            for (var i = 0; i < count; i++)
            {
                await writer.WriteAsync(i);
                await Task.Delay(delay);
            }

            writer.TryComplete();
        }
    }
}
